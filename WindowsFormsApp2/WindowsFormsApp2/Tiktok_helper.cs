using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WindowsFormsApp2
{
    internal class Tiktok_helper
    {
        private static readonly Random rd = new Random();

        public static string GetUsernameFromXml(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) return "";

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);

                var nodes = doc.GetElementsByTagName("node");

                foreach (XmlNode node in nodes)
                {
                    var textAttr = node.Attributes?["text"]?.Value;
                    if (!string.IsNullOrEmpty(textAttr) && textAttr.StartsWith("@") && textAttr.Length > 1)
                    {
                        string u = textAttr.TrimStart('@').Trim();
                        if (!long.TryParse(u, out _))
                        {
                            return u;
                        }
                    }

                    var descAttr = node.Attributes?["content-desc"]?.Value;
                    if (!string.IsNullOrEmpty(descAttr) && descAttr.StartsWith("@") && descAttr.Length > 1)
                    {
                        string u = descAttr.TrimStart('@').Trim();
                        if (!long.TryParse(u, out _))
                        {
                            return u;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi parse XML Username: " + ex.Message);
            }

            return "";
        }

        public static string Get2FAKeyFromXml(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) return "";

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);

                var nodes = doc.GetElementsByTagName("node");

                // Ưu tiên 1: Tìm node có resource-id chứa 'zjk' hoặc 'zaq' (ID chuẩn của ô key trên TikTok)
                foreach (XmlNode node in nodes)
                {
                    string resId = node.Attributes?["resource-id"]?.Value ?? "";
                    string text = node.Attributes?["text"]?.Value ?? "";
                    if ((resId.EndsWith(":id/zjk") || resId.EndsWith(":id/zaq") || resId.EndsWith(":id/zo4")) && !string.IsNullOrWhiteSpace(text))
                    {
                        string clean = text.Replace(" ", "").Replace("-", "").Trim().ToUpper();
                        if (clean.Length >= 16 && !IsInvalidKey(clean))
                        {
                            return clean;
                        }
                    }
                }

                // Ưu tiên 2: Tìm node đứng cạnh / ngay trước nút "Copy key"
                for (int i = 0; i < nodes.Count; i++)
                {
                    XmlNode node = nodes[i];
                    string text = node.Attributes?["text"]?.Value ?? "";
                    if (text.Equals("Copy key", StringComparison.OrdinalIgnoreCase))
                    {
                        for (int j = i - 1; j >= Math.Max(0, i - 4); j--)
                        {
                            string prevText = nodes[j].Attributes?["text"]?.Value ?? "";
                            string clean = prevText.Replace(" ", "").Replace("-", "").Trim().ToUpper();
                            if (clean.Length >= 16 && !IsInvalidKey(clean) && clean.All(c => (c >= 'A' && c <= 'Z') || (c >= '2' && c <= '7')))
                            {
                                return clean;
                            }
                        }
                    }
                }

                // Ưu tiên 3: Tìm node có text Base32 hợp lệ (độ dài >= 16 ký tự, các ký tự thuộc A-Z, 2-7)
                foreach (XmlNode node in nodes)
                {
                    string text = node.Attributes?["text"]?.Value;
                    if (!string.IsNullOrEmpty(text))
                    {
                        string clean = text.Replace(" ", "").Replace("-", "").Trim().ToUpper();
                        if (clean.Length >= 16 && clean.All(c => (c >= 'A' && c <= 'Z') || (c >= '2' && c <= '7')))
                        {
                            if (!IsInvalidKey(clean))
                            {
                                return clean;
                            }
                        }
                    }
                }

                // Ưu tiên 4: Regex quét chuỗi Base32 trong text của XML
                var matches = System.Text.RegularExpressions.Regex.Matches(xml, @"text=""([A-Z2-7]{16,64})""", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    if (match.Success)
                    {
                        string clean = match.Groups[1].Value.Trim().ToUpper();
                        if (!IsInvalidKey(clean))
                        {
                            return clean;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi parse XML 2FA: " + ex.Message);
            }

            return "";
        }

        private static bool IsInvalidKey(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return true;
            string upper = text.ToUpperInvariant();
            return upper.Contains("STEP") ||
                   upper.Contains("VERIF") ||
                   upper.Contains("SETUP") ||
                   upper.Contains("AUTHENTIC") ||
                   upper.Contains("METHOD") ||
                   upper.Contains("TIKTOK") ||
                   upper.Contains("PHONE") ||
                   upper.Contains("EMAIL") ||
                   upper.Contains("PASS") ||
                   upper.Contains("COPY") ||
                   upper.Contains("INSTALL") ||
                   upper.Contains("SCAN") ||
                   upper.Contains("NEXT");
        }

        public static string GenerateTOTP(string secret)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(secret))
                    return "";

                secret = secret.Replace(" ", "").Replace("-", "").Trim().ToUpper();
                byte[] key = Base32Decode(secret);

                long timestep = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 30;
                byte[] timestepBytes = BitConverter.GetBytes(timestep);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(timestepBytes);

                using (var hmac = new HMACSHA1(key))
                {
                    byte[] hash = hmac.ComputeHash(timestepBytes);
                    int offset = hash[hash.Length - 1] & 0x0F;

                    int binary =
                        ((hash[offset] & 0x7F) << 24) |
                        ((hash[offset + 1] & 0xFF) << 16) |
                        ((hash[offset + 2] & 0xFF) << 8) |
                        (hash[offset + 3] & 0xFF);

                    int otp = binary % 1000000;
                    return otp.ToString("D6");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi tính OTP: " + ex.Message);
                return "";
            }
        }

        public static byte[] Base32Decode(string base32)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

            if (string.IsNullOrWhiteSpace(base32))
                return new byte[0];

            base32 = base32.TrimEnd('=').Replace(" ", "").Replace("-", "").ToUpper();

            List<byte> bytes = new List<byte>();
            int bitBuffer = 0;
            int bitCount = 0;

            foreach (char c in base32)
            {
                int value = alphabet.IndexOf(c);
                if (value < 0) continue;

                bitBuffer = (bitBuffer << 5) | value;
                bitCount += 5;

                if (bitCount >= 8)
                {
                    bytes.Add((byte)((bitBuffer >> (bitCount - 8)) & 0xFF));
                    bitCount -= 8;
                }
            }

            return bytes.ToArray();
        }

        private static readonly string[] firstNames = {
            "James", "John", "Robert", "Michael", "William", "David", "Richard", "Joseph", "Thomas", "Charles",
            "Christopher", "Daniel", "Matthew", "Anthony", "Donald", "Mark", "Paul", "Steven", "Andrew", "Kenneth",
            "Joshua", "Kevin", "Brian", "George", "Edward", "Ronald", "Timothy", "Jason", "Jeffrey", "Ryan",
            "Jacob", "Gary", "Nicholas", "Eric", "Jonathan", "Stephen", "Larry", "Justin", "Scott", "Brandon",
            "Benjamin", "Samuel", "Gregory", "Alexander", "Frank", "Patrick", "Raymond", "Jack", "Dennis", "Jerry",
            "Tyler", "Aaron", "Jose", "Adam", "Nathan", "Henry", "Douglas", "Zachary", "Peter", "Kyle",
            "Walter", "Ethan", "Jeremy", "Harold", "Keith", "Christian", "Roger", "Noah", "Gerald", "Carl",
            "Terry", "Sean", "Austin", "Arthur", "Lawrence", "Jesse", "Dylan", "Bryan", "Joe", "Jordan",
            "Billy", "Bruce", "Albert", "Willie", "Gabriel", "Logan", "Alan", "Juan", "Wayne", "Roy",
            "Ralph", "Randy", "Eugene", "Vincent", "Russell", "Louis", "Philip", "Bobby", "Johnny", "Bradley",
            "Mary", "Patricia", "Jennifer", "Linda", "Elizabeth", "Barbara", "Susan", "Jessica", "Sarah", "Karen",
            "Lisa", "Nancy", "Betty", "Margaret", "Sandra", "Ashley", "Kimberly", "Emily", "Donna", "Michelle",
            "Carol", "Amanda", "Melissa", "Deborah", "Stephanie", "Rebecca", "Sharon", "Laura", "Cynthia", "Kathleen",
            "Amy", "Angela", "Shirley", "Anna", "Brenda", "Pamela", "Emma", "Nicole", "Helen", "Samantha",
            "Katherine", "Christine", "Debra", "Rachel", "Carolyn", "Janet", "Catherine", "Maria", "Heather", "Diane",
            "Ruth", "Julie", "Olivia", "Joyce", "Virginia", "Victoria", "Kelly", "Lauren", "Christina", "Joan",
            "Evelyn", "Judith", "Megan", "Andrea", "Cheryl", "Hannah", "Jacqueline", "Martha", "Gloria", "Teresa",
            "Ann", "Sara", "Madison", "Frances", "Kathryn", "Janice", "Jean", "Abigail", "Alice", "Julia",
            "Judy", "Sophia", "Grace", "Denise", "Amber", "Doris", "Marilyn", "Danielle", "Beverly", "Charlotte",
            "Theresa", "Diana", "Brittany", "Natalie", "Isabella", "Marie", "Kayla", "Alexis", "Lori", "Tiffany"
        };

        private static readonly string[] lastNames = {
            "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
            "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin",
            "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson",
            "Walker", "Young", "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores",
            "Green", "Adams", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell", "Carter", "Roberts",
            "Gomez", "Phillips", "Evans", "Turner", "Diaz", "Parker", "Cruz", "Edwards", "Collins", "Reyes",
            "Stewart", "Morris", "Morales", "Murphy", "Cook", "Rogers", "Gutierrez", "Ortiz", "Morgan", "Cooper",
            "Peterson", "Bailey", "Reed", "Kelly", "Howard", "Ramos", "Kim", "Cox", "Ward", "Richardson",
            "Watson", "Brooks", "Chavez", "Wood", "James", "Bennett", "Gray", "Mendoza", "Ruiz", "Hughes",
            "Price", "Alvarez", "Castillo", "Sanders", "Patel", "Myers", "Long", "Ross", "Foster", "Jimenez"
        };

        public static string GenerateRandomNickname()
        {
            string first = firstNames[rd.Next(firstNames.Length)];
            string last = lastNames[rd.Next(lastNames.Length)];
            return $"{first} {last}";
        }
    }
}
