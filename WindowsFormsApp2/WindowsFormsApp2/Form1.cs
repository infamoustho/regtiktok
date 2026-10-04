using KAutoHelper;
using NeoX;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Infamous;
using System.Security.Cryptography;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        private static readonly SemaphoreSlim mailRequestLock = new SemaphoreSlim(1, 1);
        private static readonly HashSet<string> usedMail = new HashSet<string>();
        private readonly object deviceLock = new object();
        private Dictionary<string, CancellationTokenSource> deviceTokens = new Dictionary<string, CancellationTokenSource>();
        private Dictionary<string, DateTime> deviceStartTime = new Dictionary<string, DateTime>();
        public Form1()
        {
            InitializeComponent();
        }
        #region data
        //chỗ này là bitmap
        Bitmap ggloin, bsn, signin, iage, iage_new, i_understand, google_pw_error, securityy, ttlog, phone, news, capchalogin, accept, seting, seting1, setinggiua, update, setingtren, welcome, stepveri, skip2, capcha, hoso, skipphone, google_error, create, mat, security, tich, st1, verymail, basoc, google, tieptuc, pass_welcome, pass_show;
        void LoadData()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            ggloin = LoadBitmap(Path.Combine(basePath, "Data/ggicon1.png"));
            bsn = LoadBitmap(Path.Combine(basePath, "Data/banhsinhnhat.png"));
            signin = LoadBitmap(Path.Combine(basePath, "Data/signin.png"));
            iage = LoadBitmap(Path.Combine(basePath, "Data/iage.png"));
            iage_new = LoadBitmap(Path.Combine(basePath, "Data/iage_new.png"));
            i_understand = LoadBitmap(Path.Combine(basePath, "Data/i_understand.png"));
            google_pw_error = LoadBitmap(Path.Combine(basePath, "Data/google_pw_error.png"));
            accept = LoadBitmap(Path.Combine(basePath, "Data/accept.png"));
            skip2 = LoadBitmap(Path.Combine(basePath, "Data/skip2.png"));
            hoso = LoadBitmap(Path.Combine(basePath, "Data/hoso.png"));
            create = LoadBitmap(Path.Combine(basePath, "Data/create.png"));
            mat = LoadBitmap(Path.Combine(basePath, "Data/mat.png"));
            security = LoadBitmap(Path.Combine(basePath, "Data/security.png"));
            tich = LoadBitmap(Path.Combine(basePath, "Data/tich.png"));
            st1 = LoadBitmap(Path.Combine(basePath, "Data/st1.png"));
            verymail = LoadBitmap(Path.Combine(basePath, "Data/verymail.png"));
            basoc = LoadBitmap(Path.Combine(basePath, "Data/basoc.png"));
            google = LoadBitmap(Path.Combine(basePath, "Data/google.png"));
            tieptuc = LoadBitmap(Path.Combine(basePath, "Data/tieptuc.png"));
            skipphone = LoadBitmap(Path.Combine(basePath, "Data/skipphone.png"));
            google_error = LoadBitmap(Path.Combine(basePath, "Data/google_error.png"));
            capcha = LoadBitmap(Path.Combine(basePath, "Data/capcha.png"));
            welcome = LoadBitmap(Path.Combine(basePath, "Data/welcome.png"));
            ttlog = LoadBitmap(Path.Combine(basePath, "Data/ttlog.png"));
            stepveri = LoadBitmap(Path.Combine(basePath, "Data/stepveri.png"));
            securityy = LoadBitmap(Path.Combine(basePath, "Data/security1.png"));
            setinggiua = LoadBitmap(Path.Combine(basePath, "Data/setinggiua.png"));
            setingtren = LoadBitmap(Path.Combine(basePath, "Data/setingtren.png"));
            seting = LoadBitmap(Path.Combine(basePath, "Data/seting.png"));
            seting1 = LoadBitmap(Path.Combine(basePath, "Data/st1.png"));
            phone = LoadBitmap(Path.Combine(basePath, "Data/phone.png"));
            update = LoadBitmap(Path.Combine(basePath, "Data/update.png"));
            capchalogin = LoadBitmap(Path.Combine(basePath, "Data/capcha1.png"));
            news = LoadBitmap(Path.Combine(basePath, "Data/news.png"));
            pass_welcome = LoadBitmap(Path.Combine(basePath, "Data/pass_welcome.png"));
            pass_show = LoadBitmap(Path.Combine(basePath, "Data/pass_show.png"));
        }
        Bitmap LoadBitmap(string path)
        {
            using (var temp = (Bitmap)Bitmap.FromFile(path))
            {
                return new Bitmap(temp); // đã tách file
            }
        }
        //Chỗ này là các nút trên form
        private void label3_Click(object sender, EventArgs e)
        {

        }

        private static readonly AsyncLocal<string> currentDeviceID = new AsyncLocal<string>();
        private readonly ConcurrentDictionary<string, ManualResetEventSlim> devicePauseEvents = new ConcurrentDictionary<string, ManualResetEventSlim>();
        private readonly ConcurrentDictionary<string, bool> devicePausedStatus = new ConcurrentDictionary<string, bool>();
        private readonly ManualResetEventSlim pauseEvent = new ManualResetEventSlim(true); // Fallback toàn cục

        private string GetSelectedDeviceID()
        {
            try
            {
                if (dgvDevices.InvokeRequired)
                {
                    return (string)dgvDevices.Invoke(new Func<string>(GetSelectedDeviceID));
                }

                if (dgvDevices.SelectedRows.Count > 0)
                {
                    string id = dgvDevices.SelectedRows[0].Cells[0].Value?.ToString();
                    if (!string.IsNullOrWhiteSpace(id)) return id.Trim();
                }

                if (dgvDevices.SelectedCells.Count > 0)
                {
                    int rowIndex = dgvDevices.SelectedCells[0].RowIndex;
                    if (rowIndex >= 0 && rowIndex < dgvDevices.Rows.Count)
                    {
                        string id = dgvDevices.Rows[rowIndex].Cells[0].Value?.ToString();
                        if (!string.IsNullOrWhiteSpace(id)) return id.Trim();
                    }
                }

                if (dgvDevices.CurrentRow != null)
                {
                    string id = dgvDevices.CurrentRow.Cells[0].Value?.ToString();
                    if (!string.IsNullOrWhiteSpace(id)) return id.Trim();
                }
            }
            catch { }
            return null;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            string deviceID = GetSelectedDeviceID();
            if (string.IsNullOrEmpty(deviceID))
            {
                MessageBox.Show("Vui lòng chọn thiết bị trong bảng để tạm dừng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            lock (deviceLock)
            {
                if (!deviceTokens.ContainsKey(deviceID))
                {
                    UpdateProcess(deviceID + " ⚠️ Thiết bị này hiện không chạy!");
                    return;
                }
            }

            var evt = devicePauseEvents.GetOrAdd(deviceID, _ => new ManualResetEventSlim(true));
            if (!evt.IsSet)
            {
                UpdateProcess(deviceID + " ⚠️ Thiết bị này đã ở trạng thái tạm dừng rồi!");
                return;
            }

            evt.Reset(); // Tạm dừng độc lập cho thiết bị này
            devicePausedStatus[deviceID] = true;
            UpdateProcess(deviceID + " ⏸ Đang tạm dừng");
            UpdateDeviceLog(deviceID, "⏸ Đang tạm dừng");
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            string deviceID = GetSelectedDeviceID();
            if (string.IsNullOrEmpty(deviceID))
            {
                MessageBox.Show("Vui lòng chọn thiết bị đang tạm dừng để tiếp tục!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            lock (deviceLock)
            {
                if (!deviceTokens.ContainsKey(deviceID))
                {
                    UpdateProcess(deviceID + " ⚠️ Thiết bị này hiện không chạy!");
                    return;
                }
            }

            if (!devicePauseEvents.TryGetValue(deviceID, out var evt) || evt.IsSet)
            {
                UpdateProcess(deviceID + " ⚠️ Thiết bị này đang chạy bình thường, không ở trạng thái tạm dừng!");
                return;
            }

            evt.Set(); // Đánh thức luồng của thiết bị để chạy tiếp từ bước đang làm
            devicePausedStatus[deviceID] = false;
            UpdateProcess(deviceID + " ▶️ Tiếp tục chạy từ bước đang làm");
            UpdateDeviceLog(deviceID, "▶️ Tiếp tục chạy");
        }

        private void btnStopAll_Click(object sender, EventArgs e)
        {
            lock (deviceLock)
            {
                foreach (var item in deviceTokens.Values)
                {
                    try { item.Cancel(); } catch { }
                }

                deviceTokens.Clear();
                deviceStartTime.Clear();

                // Đánh thức toàn bộ các thread đang bị tạm dừng để thoát ra sạch sẽ
                foreach (var evt in devicePauseEvents.Values)
                {
                    try { evt.Set(); } catch { }
                }
                devicePausedStatus.Clear();
                pauseEvent.Set();
            }

            isRunning = false;
            if (button1.InvokeRequired)
            {
                button1.Invoke(new Action(() => button1.Enabled = true));
            }
            else
            {
                button1.Enabled = true;
            }

            UpdateProcess("⏹ Đã dừng toàn bộ các devices đang chạy");

            foreach (DataGridViewRow row in dgvDevices.Rows)
            {
                string dev = row.Cells[0].Value?.ToString();
                if (!string.IsNullOrEmpty(dev))
                {
                    UpdateDeviceLog(dev, "⏹ Đã dừng");
                }
            }
        }

        private void processText_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnReloadDevices_Click(object sender, EventArgs e)
        {
            LoadDevices();
            UpdateProcess("🔄 Đã load lại danh sách device");
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            string deviceID = GetSelectedDeviceID();
            if (string.IsNullOrEmpty(deviceID))
            {
                MessageBox.Show("Vui lòng chọn thiết bị trong bảng để Reset!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 1. Dừng luồng cũ của thiết bị này nếu đang chạy hoặc đang pause
            lock (deviceLock)
            {
                if (deviceTokens.TryGetValue(deviceID, out var oldCts))
                {
                    UpdateProcess(deviceID + " 🛑 Dừng tiến trình cũ để chuẩn bị Reset...");
                    try { oldCts.Cancel(); } catch { }
                    if (devicePauseEvents.TryGetValue(deviceID, out var pEvt))
                    {
                        try { pEvt.Set(); } catch { }
                    }
                    devicePausedStatus[deviceID] = false;
                    deviceTokens.Remove(deviceID);
                    deviceStartTime.Remove(deviceID);
                }
            }

            // 2. Chạy quy trình Reset không đồng bộ trên background: Sign out ExpressVPN -> Change Device -> Chạy lại từ đầu
            Task.Run(async () =>
            {
                await Task.Delay(1500); // Chờ luồng cũ dừng hoàn toàn

                var resetCts = new CancellationTokenSource();
                lock (deviceLock)
                {
                    deviceTokens[deviceID] = resetCts;
                    deviceStartTime[deviceID] = DateTime.Now;
                }

                currentDeviceID.Value = deviceID;

                try
                {
                    UpdateProcess(deviceID + " 🔄 [RESET] Bắt đầu quy trình Reset thiết bị...");
                    // BƯỚC 1: Sign out ExpressVPN (Nếu dùng ExpressVPN)
                    if (SelectedVpnProvider == "ExpressVPN")
                    {
                        UpdateDeviceLog(deviceID, "🔄 Đang Sign Out ExpressVPN...");
                        UpdateProcess(deviceID + " 🔄 [RESET] Bước 1: Mở ExpressVPN để Sign Out...");
                        await SignOutExpressVpnAsync(deviceID, resetCts.Token);
                        UpdateProcess(deviceID + " ✅ [RESET] Đã hoàn tất Sign Out ExpressVPN!");
                    }
                    else
                    {
                        UpdateProcess(deviceID + " 🔄 [RESET] Đang dùng HMA VPN: Bỏ qua Sign Out.");
                    }

                    // BƯỚC 2: Gọi Change Device (US, Android 15)
                    UpdateProcess(deviceID + " 🔄 [RESET] Bước 2: Đang gọi API Change Device (US, Android 15)...");
                    UpdateDeviceLog(deviceID, "🔄 Đang Change Device...");
                    bool changed = await ChangeDeviceAsync(deviceID, resetCts.Token);
                    if (changed)
                    {
                        UpdateProcess(deviceID + " ✅ [RESET] Thiết bị đã Change Device thành công & mạng sẵn sàng!");
                    }
                    else
                    {
                        UpdateProcess(deviceID + " ⚠️ [RESET] Change Device chưa phản hồi thành công, nhưng vẫn chuẩn bị chạy lại.");
                    }

                    // BƯỚC 3: Chạy lại từ đầu
                    int devIndex = 0;
                    var devices = KAutoHelper.ADBHelper.GetDevices();
                    int idx = devices.IndexOf(deviceID);
                    if (idx >= 0) devIndex = idx;

                    UpdateProcess(deviceID + " 🚀 [RESET] Bước 3: Bắt đầu chạy lại thiết bị từ đầu...");
                    UpdateDeviceLog(deviceID, "🚀 Bắt đầu chạy");

                    lock (deviceLock)
                    {
                        deviceTokens.Remove(deviceID);
                        deviceStartTime.Remove(deviceID);
                    }

                    StartDevice(deviceID, devIndex);
                }
                catch (OperationCanceledException)
                {
                    UpdateProcess(deviceID + " ⏹ [RESET] Tiến trình Reset đã bị dừng.");
                    UpdateDeviceLog(deviceID, "⏹ Đã dừng");
                    lock (deviceLock)
                    {
                        deviceTokens.Remove(deviceID);
                        deviceStartTime.Remove(deviceID);
                    }
                }
                catch (Exception ex)
                {
                    UpdateProcess(deviceID + " ❌ [RESET] Lỗi khi Reset: " + ex.Message);
                    UpdateDeviceLog(deviceID, "❌ Lỗi Reset");
                    lock (deviceLock)
                    {
                        deviceTokens.Remove(deviceID);
                        deviceStartTime.Remove(deviceID);
                    }
                }
            });
        }

        void CheckPause(CancellationToken token, string devId = null)
        {
            if (string.IsNullOrEmpty(devId))
                devId = currentDeviceID.Value;

            ManualResetEventSlim evt = null;
            if (!string.IsNullOrEmpty(devId))
            {
                devicePauseEvents.TryGetValue(devId, out evt);
            }
            if (evt == null)
            {
                evt = pauseEvent;
            }

            if (!evt.IsSet)
            {
                evt.Wait(token);
            }
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                File.WriteAllText("expressvpn.txt", txtExpressVpn.Text.Trim());
            }
            catch { }
        }
        private readonly Queue<string> emailQueue = new Queue<string>();
        private readonly object emailQueueLock = new object();
        private string selectedEmailFilePath = null;

        void LoadEmailQueue()
        {
            lock (emailQueueLock)
            {
                emailQueue.Clear();
                string content = "";
                if (txtEmails.InvokeRequired)
                {
                    content = (string)txtEmails.Invoke(new Func<string>(() => txtEmails.Text));
                }
                else
                {
                    content = txtEmails.Text;
                }

                var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    string clean = line.Trim();
                    if (!string.IsNullOrWhiteSpace(clean))
                        emailQueue.Enqueue(clean);
                }

                if (emailQueue.Count == 0 && string.IsNullOrEmpty(selectedEmailFilePath))
                {
                    if (labelEmails.InvokeRequired)
                        labelEmails.BeginInvoke(new Action(() => labelEmails.Text = "Danh sách Email (email|pass):"));
                    else
                        labelEmails.Text = "Danh sách Email (email|pass):";
                }

                UpdateProcess($"📧 Đã nạp {emailQueue.Count} email vào hàng đợi");
            }
        }

        string GetNextEmail()
        {
            lock (emailQueueLock)
            {
                if (emailQueue.Count > 0)
                {
                    string acc = emailQueue.Dequeue();

                    // 1. Cập nhật TextBox: Xóa dòng email vừa lấy
                    try
                    {
                        string remainingText = string.Join(Environment.NewLine, emailQueue);
                        if (txtEmails.InvokeRequired)
                        {
                            txtEmails.BeginInvoke(new Action(() =>
                            {
                                txtEmails.Text = remainingText;
                            }));
                        }
                        else
                        {
                            txtEmails.Text = remainingText;
                        }
                    }
                    catch { }

                    // 2. Cập nhật File .txt: Xóa dòng email vừa lấy khỏi file txt
                    try
                    {
                        if (!string.IsNullOrEmpty(selectedEmailFilePath) && File.Exists(selectedEmailFilePath))
                        {
                            File.WriteAllLines(selectedEmailFilePath, emailQueue);
                        }
                    }
                    catch (Exception ex)
                    {
                        UpdateProcess("⚠️ Không thể cập nhật file email: " + ex.Message);
                    }

                    return acc;
                }
                return null;
            }
        }

        bool HasRemainingEmails()
        {
            lock (emailQueueLock)
            {
                return emailQueue.Count > 0;
            }
        }

        #region GmailVIP Auto Buy System
        private readonly SemaphoreSlim autoBuyLock = new SemaphoreSlim(1, 1);

        private void InitGmailVipControls()
        {
            try
            {
                // 1. Thêm các gói email thông dụng mặc định
                cboGmailVipProduct.Items.Clear();
                cboGmailVipProduct.Items.Add(new GmailVipProductItem { Id = 1, Name = "Gmail Domain 2h-12h", Price = 36, CategoryName = "Gmail Domain" });
                cboGmailVipProduct.Items.Add(new GmailVipProductItem { Id = 6, Name = "Gmail Domain 12h-48h", Price = 69, CategoryName = "Gmail Domain" });
                cboGmailVipProduct.Items.Add(new GmailVipProductItem { Id = 7, Name = "Gmail Domain Loại B", Price = 29, CategoryName = "Gmail Domain" });
                cboGmailVipProduct.Items.Add(new GmailVipProductItem { Id = 1301, Name = "Hotmail Trusted OAuth2", Price = 237, CategoryName = "Hotmail" });
                cboGmailVipProduct.Items.Add(new GmailVipProductItem { Id = 2050, Name = "Gmail Domain .us 24h-48h", Price = 233, CategoryName = "Gmail Domain" });

                // 2. Load cấu hình đã lưu
                var config = GmailVipService.LoadConfig();
                txtGmailVipApiKey.Text = config.ApiKey ?? "";
                numBuyAmount.Value = Math.Max(1, Math.Min(500, config.Amount > 0 ? config.Amount : 5));
                chkAutoBuy.Checked = config.AutoBuy;

                // Chọn gói theo ProductId
                int selectIdx = 0;
                for (int i = 0; i < cboGmailVipProduct.Items.Count; i++)
                {
                    if (cboGmailVipProduct.Items[i] is GmailVipProductItem item && item.Id == config.ProductId)
                    {
                        selectIdx = i;
                        break;
                    }
                }
                if (cboGmailVipProduct.Items.Count > 0)
                {
                    cboGmailVipProduct.SelectedIndex = selectIdx;
                }

                // Cập nhật DropDownWidth và hiển thị chi tiết gói
                AdjustProductDropDownWidth();
                UpdateProductDetailDisplay();

                // Style hover cho các nút
                SetupButtonHover(btnCheckBalance, Color.FromArgb(238, 242, 255), Color.FromArgb(224, 231, 255));
                SetupButtonHover(btnBuyNow, Color.FromArgb(79, 70, 229), Color.FromArgb(67, 56, 202));
                SetupButtonHover(btnLoadProducts, Color.FromArgb(241, 245, 249), Color.FromArgb(226, 232, 240));

                // Tự động kiểm tra số dư nếu đã có API Key
                if (!string.IsNullOrWhiteSpace(txtGmailVipApiKey.Text))
                {
                    _ = CheckGmailVipBalanceAsync(silent: true);
                }
            }
            catch { }
        }

        private void SaveGmailVipConfig()
        {
            try
            {
                int productId = 1;
                if (cboGmailVipProduct.SelectedItem is GmailVipProductItem selItem)
                {
                    productId = selItem.Id;
                }

                var config = new GmailVipConfig
                {
                    ApiKey = txtGmailVipApiKey.Text.Trim(),
                    ProductId = productId,
                    Amount = (int)numBuyAmount.Value,
                    AutoBuy = chkAutoBuy.Checked
                };
                GmailVipService.SaveConfig(config);
            }
            catch { }
        }

        private async Task<bool> CheckGmailVipBalanceAsync(bool silent = false)
        {
            string apiKey = txtGmailVipApiKey.Text.Trim();
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                if (!silent)
                {
                    MessageBox.Show("Vui lòng nhập API Key của GmailVIP trước!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                lblGmailVipBalance.Text = "💰 Số dư: Chưa nhập Key";
                lblGmailVipBalance.ForeColor = Color.FromArgb(239, 68, 68);
                return false;
            }

            lblGmailVipBalance.Text = "💰 Số dư: Đang kiểm tra...";
            lblGmailVipBalance.ForeColor = Color.FromArgb(100, 116, 139);

            var result = await GmailVipService.GetProfileAsync(apiKey);
            if (result.Success)
            {
                lblGmailVipBalance.Text = $"💰 Số dư: {result.Money:N0} đ";
                lblGmailVipBalance.ForeColor = Color.FromArgb(16, 185, 129);
                if (!silent)
                {
                    UpdateProcess($"💳 [GmailVIP] Tài khoản: {result.Username} | Số dư hiện tại: {result.Money:N0} đ");
                }
                return true;
            }
            else
            {
                lblGmailVipBalance.Text = $"⚠️ Lỗi: {result.Message}";
                lblGmailVipBalance.ForeColor = Color.FromArgb(239, 68, 68);
                if (!silent)
                {
                    UpdateProcess($"⚠️ [GmailVIP] Kiểm tra số dư thất bại: {result.Message}");
                }
                return false;
            }
        }

        public async Task<bool> TryAutoBuyEmailsAsync(string deviceID = null)
        {
            await autoBuyLock.WaitAsync();
            try
            {
                // Kiểm tra lại xem đã có email nào được nạp bởi luồng khác chưa
                if (emailQueue.Count > 1)
                {
                    return true;
                }

                bool isAutoEnabled = false;
                string apiKey = "";
                int productId = 1;
                int amount = 5;

                if (chkAutoBuy.InvokeRequired)
                {
                    chkAutoBuy.Invoke(new Action(() =>
                    {
                        isAutoEnabled = chkAutoBuy.Checked;
                        apiKey = txtGmailVipApiKey.Text.Trim();
                        if (cboGmailVipProduct.SelectedItem is GmailVipProductItem item)
                            productId = item.Id;
                        amount = (int)numBuyAmount.Value;
                    }));
                }
                else
                {
                    isAutoEnabled = chkAutoBuy.Checked;
                    apiKey = txtGmailVipApiKey.Text.Trim();
                    if (cboGmailVipProduct.SelectedItem is GmailVipProductItem item)
                        productId = item.Id;
                    amount = (int)numBuyAmount.Value;
                }

                if (!isAutoEnabled)
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    UpdateProcess("⚠️ [GmailVIP AutoBuy] Chưa cấu hình API Key!");
                    return false;
                }

                UpdateProcess($"🛒 [GmailVIP] Đang tự động mua thêm {amount} email (Gói ID: {productId})...");
                if (!string.IsNullOrEmpty(deviceID))
                {
                    UpdateDeviceLog(deviceID, $"⚡ Đang mua {amount} email tự động...");
                }

                var buyResult = await GmailVipService.BuyEmailAsync(apiKey, productId, amount);
                if (buyResult.Success && buyResult.Data != null && buyResult.Data.Count > 0)
                {
                    lock (emailQueueLock)
                    {
                        foreach (var mail in buyResult.Data)
                        {
                            string clean = mail.Trim();
                            if (!string.IsNullOrWhiteSpace(clean))
                            {
                                emailQueue.Enqueue(clean);
                            }
                        }

                        // Cập nhật giao diện txtEmails
                        string allRemaining = string.Join(Environment.NewLine, emailQueue);
                        if (txtEmails.InvokeRequired)
                        {
                            txtEmails.BeginInvoke(new Action(() => txtEmails.Text = allRemaining));
                        }
                        else
                        {
                            txtEmails.Text = allRemaining;
                        }

                        // Lưu vào file email
                        try
                        {
                            string targetFile = selectedEmailFilePath;
                            if (string.IsNullOrEmpty(targetFile) || !File.Exists(targetFile))
                            {
                                targetFile = "emails.txt";
                            }
                            File.AppendAllLines(targetFile, buyResult.Data);
                        }
                        catch { }
                    }

                    UpdateProcess($"✅ [GmailVIP] Đã tự động mua thành công {buyResult.Data.Count} email! Mã GD: {buyResult.TransId} (Tổng hàng đợi: {emailQueue.Count})");
                    if (!string.IsNullOrEmpty(deviceID))
                    {
                        UpdateDeviceLog(deviceID, $"✅ Đã mua thêm {buyResult.Data.Count} email");
                    }

                    _ = CheckGmailVipBalanceAsync(silent: true);
                    return true;
                }
                else
                {
                    UpdateProcess($"❌ [GmailVIP] Tự động mua email thất bại: {buyResult.Message}");
                    if (!string.IsNullOrEmpty(deviceID))
                    {
                        UpdateDeviceLog(deviceID, $"❌ Mua mail thất bại: {buyResult.Message}");
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                UpdateProcess("❌ [GmailVIP] Lỗi hệ thống tự mua: " + ex.Message);
                return false;
            }
            finally
            {
                autoBuyLock.Release();
            }
        }

        private async void btnCheckBalance_Click(object sender, EventArgs e)
        {
            btnCheckBalance.Enabled = false;
            try
            {
                await CheckGmailVipBalanceAsync(silent: false);
            }
            finally
            {
                btnCheckBalance.Enabled = true;
            }
        }

        private async void btnBuyNow_Click(object sender, EventArgs e)
        {
            string apiKey = txtGmailVipApiKey.Text.Trim();
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                MessageBox.Show("Vui lòng nhập API Key trước khi mua!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int productId = 1;
            string productName = "Email";
            if (cboGmailVipProduct.SelectedItem is GmailVipProductItem item)
            {
                productId = item.Id;
                productName = item.Name;
            }

            int amount = (int)numBuyAmount.Value;
            if (MessageBox.Show($"Xác nhận mua ngay {amount} tài khoản [{productName}] từ GmailVIP?", "Xác nhận mua hàng", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            btnBuyNow.Enabled = false;
            btnBuyNow.Text = "⏳ Đang mua...";

            try
            {
                var buyResult = await GmailVipService.BuyEmailAsync(apiKey, productId, amount);
                if (buyResult.Success && buyResult.Data != null && buyResult.Data.Count > 0)
                {
                    lock (emailQueueLock)
                    {
                        foreach (var mail in buyResult.Data)
                        {
                            string clean = mail.Trim();
                            if (!string.IsNullOrWhiteSpace(clean))
                            {
                                emailQueue.Enqueue(clean);
                            }
                        }

                        string allRemaining = string.Join(Environment.NewLine, emailQueue);
                        txtEmails.Text = allRemaining;

                        try
                        {
                            string targetFile = selectedEmailFilePath;
                            if (string.IsNullOrEmpty(targetFile) || !File.Exists(targetFile))
                            {
                                targetFile = "emails.txt";
                            }
                            File.AppendAllLines(targetFile, buyResult.Data);
                        }
                        catch { }
                    }

                    UpdateProcess($"🎉 [GmailVIP] Mua thành công {buyResult.Data.Count} email! Mã GD: {buyResult.TransId}");
                    MessageBox.Show($"Mua thành công {buyResult.Data.Count} email và đã nạp vào hàng đợi!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _ = CheckGmailVipBalanceAsync(silent: true);
                }
                else
                {
                    UpdateProcess($"❌ [GmailVIP] Mua hàng thất bại: {buyResult.Message}");
                    MessageBox.Show($"Mua hàng thất bại:\n{buyResult.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                btnBuyNow.Enabled = true;
                btnBuyNow.Text = "🛒 Mua ngay";
            }
        }

        private async void btnLoadProducts_Click(object sender, EventArgs e)
        {
            string apiKey = txtGmailVipApiKey.Text.Trim();
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                MessageBox.Show("Vui lòng nhập API Key để tải danh sách gói!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnLoadProducts.Enabled = false;
            btnLoadProducts.Text = "⏳ Đang tải...";

            try
            {
                var products = await GmailVipService.GetProductsAsync(apiKey);
                if (products != null && products.Count > 0)
                {
                    int currentId = 1;
                    if (cboGmailVipProduct.SelectedItem is GmailVipProductItem curItem)
                        currentId = curItem.Id;

                    cboGmailVipProduct.Items.Clear();
                    int selectIdx = 0;
                    for (int i = 0; i < products.Count; i++)
                    {
                        cboGmailVipProduct.Items.Add(products[i]);
                        if (products[i].Id == currentId) selectIdx = i;
                    }

                    cboGmailVipProduct.SelectedIndex = selectIdx;
                    AdjustProductDropDownWidth();
                    UpdateProductDetailDisplay();
                    UpdateProcess($"📋 [GmailVIP] Đã tải {products.Count} gói sản phẩm từ GmailVIP");
                    MessageBox.Show($"Đã tải {products.Count} gói sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sản phẩm nào hoặc API Key không đúng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải gói: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLoadProducts.Enabled = true;
                btnLoadProducts.Text = "📋 Tải gói";
            }
        }

        private void AdjustProductDropDownWidth()
        {
            try
            {
                int maxWidth = 720;
                using (Graphics g = cboGmailVipProduct.CreateGraphics())
                {
                    foreach (var item in cboGmailVipProduct.Items)
                    {
                        if (item != null)
                        {
                            int w = (int)g.MeasureString(item.ToString(), cboGmailVipProduct.Font).Width + 40;
                            if (w > maxWidth) maxWidth = w;
                        }
                    }
                }
                cboGmailVipProduct.DropDownWidth = maxWidth;
            }
            catch { }
        }

        private void UpdateProductDetailDisplay()
        {
            try
            {
                if (cboGmailVipProduct.SelectedItem is GmailVipProductItem item)
                {
                    string stockStr = string.IsNullOrWhiteSpace(item.Stock) ? "Sẵn sàng" : item.Stock;
                    lblGmailVipProductDetail.Text = $"💵 Giá: {item.Price:N0}đ / mail  |  📦 Kho: {stockStr}";
                    toolTipGmailVip?.SetToolTip(cboGmailVipProduct, $"[ID: {item.Id}] {item.Name}\nGiá: {item.Price:N0}đ / mail\nLoại: {item.CategoryName}\nCòn lại: {stockStr}");
                }
                else
                {
                    lblGmailVipProductDetail.Text = "💵 Giá: --  |  📦 Kho: --";
                    toolTipGmailVip?.SetToolTip(cboGmailVipProduct, "Chọn gói email");
                }
            }
            catch { }
        }

        private void txtGmailVipApiKey_TextChanged(object sender, EventArgs e)
        {
            SaveGmailVipConfig();
        }

        private void cboGmailVipProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveGmailVipConfig();
            UpdateProductDetailDisplay();
        }

        private void numBuyAmount_ValueChanged(object sender, EventArgs e)
        {
            SaveGmailVipConfig();
        }

        private void chkAutoBuy_CheckedChanged(object sender, EventArgs e)
        {
            SaveGmailVipConfig();
            if (chkAutoBuy.Checked)
            {
                chkAutoBuy.ForeColor = Color.FromArgb(16, 185, 129);
                UpdateProcess("⚡ [GmailVIP] Đã bật chế độ Tự động mua email khi hết");
            }
            else
            {
                chkAutoBuy.ForeColor = Color.FromArgb(100, 116, 139);
                UpdateProcess("ℹ️ [GmailVIP] Đã tắt chế độ Tự động mua email");
            }
        }
        #endregion

        private void btnSelectEmailFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                ofd.Title = "Chọn file danh sách Email (email|pass)";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        selectedEmailFilePath = ofd.FileName;
                        txtEmails.Text = File.ReadAllText(selectedEmailFilePath);
                        labelEmails.Text = $"DS Email ({Path.GetFileName(selectedEmailFilePath)}):";
                        LoadEmailQueue();
                        UpdateProcess($"📂 Đã nạp file email: {Path.GetFileName(selectedEmailFilePath)} ({emailQueue.Count} email)");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi đọc file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }



        bool isRunning = false;
        private readonly object startLock = new object();



        private void button1_Click(object sender, EventArgs e)
        {
            lock (startLock)
            {
                if (isRunning)
                {
                    UpdateProcess("⚠️ Auto đang chạy rồi → không start lại!");
                    return;
                }

                lock (deviceLock)
                {
                    if (deviceTokens.Count > 0)
                    {
                        UpdateProcess("⚠️ Device đang chạy → không start lại!");
                        return;
                    }
                }

                isRunning = true;
                button1.Enabled = false;

                pauseEvent.Set();
                LoadDevices();
                LoadEmailQueue();
                UpdateProcess("👉 Auto bắt đầu chạy");

                try
                {
                    Auto();
                }
                catch (Exception ex)
                {
                    UpdateProcess("❌ Lỗi: " + ex.Message);
                    isRunning = false;
                    button1.Enabled = true;
                }
            }
        }
        public static string AdbCommand(string arguments)
        {
            try
            {
                string adbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "adb.exe");
                if (!File.Exists(adbPath)) adbPath = "adb";

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = adbPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process p = Process.Start(psi))
                {
                    string output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit(10000);
                    return output;
                }
            }
            catch
            {
                return null;
            }
        }

        public static string AdbShell(string deviceID, string cmd)
        {
            try
            {
                string adbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "adb.exe");
                if (!File.Exists(adbPath)) adbPath = "adb";

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = adbPath,
                    Arguments = $"-s {deviceID} shell {cmd}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process p = Process.Start(psi))
                {
                    string output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();
                    return output;
                }
            }
            catch
            {
                return null;
            }
        }

        public static void GoHome(string deviceID)
        {
            try
            {
                // Gọi lệnh về Home trực tiếp qua ADB keyevent 3 (KEYCODE_HOME), tuyệt đối không bấm trên màn hình tránh chạm camera
                AdbShell(deviceID, "input keyevent 3");
            }
            catch { }
        }

        public static void MuteDevice(string deviceID)
        {
            try
            {
                // 1. Chuyển sang chế độ Im lặng hoàn toàn (Silent mode)
                AdbShell(deviceID, "settings put global mode_ringer 0");

                // 2. Đưa toàn bộ âm lượng thông báo (ting ting), chuông gọi, hệ thống và đa phương tiện về 0
                AdbShell(deviceID, "settings put system volume_notification 0");
                AdbShell(deviceID, "settings put system volume_ring 0");
                AdbShell(deviceID, "settings put system volume_system 0");
                AdbShell(deviceID, "settings put system volume_music 0");
            }
            catch { }
        }

        void Auto()
        {
            var devices = KAutoHelper.ADBHelper.GetDevices();

            UpdateProcess("📱 Số device: " + devices.Count);

            for (int i = 0; i < devices.Count; i++)
            {
                StartDevice(devices[i], i);
            }
        }

        private Bitmap SafeScreenShoot(string deviceID)
        {
            string adbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "adb.exe");
            if (!File.Exists(adbPath)) adbPath = "adb";

            for (int attempt = 0; attempt < 3; attempt++)
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = adbPath,
                        Arguments = $"-s {deviceID} exec-out screencap -p",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    };

                    using (Process proc = Process.Start(psi))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            proc.StandardOutput.BaseStream.CopyTo(ms);
                            proc.WaitForExit(5000);
                            if (ms.Length > 0)
                            {
                                ms.Position = 0;
                                using (var temp = Image.FromStream(ms))
                                {
                                    return new Bitmap(temp);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    Thread.Sleep(200);
                }
            }
            return null;
        }

        private bool IsGoogleLoadingScreen(Bitmap bmp)
        {
            if (bmp == null) return true;
            // Kiểm tra vùng nút Next (góc dưới phải x: 1100..1320, y: 2720..2820 trên 1440x2960)
            int startX = (int)(bmp.Width * (1100.0 / 1440.0));
            int endX = (int)(bmp.Width * (1320.0 / 1440.0));
            int startY = (int)(bmp.Height * (2720.0 / 2960.0));
            int endY = (int)(bmp.Height * (2820.0 / 2960.0));
            int blueCount = 0;
            for (int x = startX; x < endX; x += 5)
            {
                for (int y = startY; y < endY; y += 5)
                {
                    Color p = bmp.GetPixel(x, y);
                    if (p.B > 190 && p.G > 160 && p.R > 120 && p.B > (p.R + 20))
                    {
                        blueCount++;
                    }
                }
            }
            // Màn hình đang load ("Checking info...") có 0 pixel xanh ở nút Next. Khi form load xong có ~820 pixel xanh.
            return blueCount < 50;
        }

        private bool IsGoogleSkipScreen(Bitmap bmp)
        {
            if (bmp == null) return false;
            // Nếu vẫn đang quay "Checking info..." thì chưa phải màn hình Skip
            if (IsGoogleLoadingScreen(bmp)) return false;

            int startX = (int)(bmp.Width * (50.0 / 1440.0));
            int endX = (int)(bmp.Width * (220.0 / 1440.0));
            int startY = (int)(bmp.Height * (2720.0 / 2960.0));
            int endY = (int)(bmp.Height * (2820.0 / 2960.0));

            int whiteCount = 0;
            for (int x = startX; x < endX; x += 4)
            {
                for (int y = startY; y < endY; y += 4)
                {
                    Color p = bmp.GetPixel(x, y);
                    if (p.R > 160 && p.G > 160 && p.B > 160)
                    {
                        whiteCount++;
                    }
                }
            }
            // Trên màn hình Sign in with ease, chữ Skip có ~90 pixels trắng; các màn hình khác là 0
            return whiteCount >= 20;
        }

        private bool IsGoogleErrorScreen(string deviceID, Bitmap errTemplate = null)
        {
            try
            {
                // Kiểm tra focus window: nếu đang ở app VPN (ExpressVPN/HMA) hoặc màn hình chọn account thì không phải Google Error
                string focus = AdbShell(deviceID, "dumpsys window | grep mCurrentFocus");
                if (!string.IsNullOrWhiteSpace(focus) && (focus.Contains("expressvpn") || focus.Contains("hidemyass") || focus.Contains("ChooseAccountActivity")))
                {
                    return false;
                }

                // 1. Kiểm tra chính xác qua focus window ErrorActivity của Google
                if (!string.IsNullOrWhiteSpace(focus) && focus.Contains("ErrorActivity"))
                {
                    return true;
                }

                // 2. Kiểm tra nhanh qua UIDump XML
                string xml = Android.GetUIDumpSafe(deviceID);
                if (!string.IsNullOrWhiteSpace(xml))
                {
                    if (xml.Contains("Couldn't sign in") ||
                        xml.Contains("problem communicating with Google servers") ||
                        xml.Contains("Try again later") ||
                        xml.Contains("Không thể đăng nhập") ||
                        xml.Contains("sự cố khi kết nối với máy chủ") ||
                        xml.Contains("sự cố khi giao tiếp với máy chủ"))
                    {
                        return true;
                    }
                }

                // 3. Chỉ kiểm tra qua template ảnh khi đang thực sự ở trong Google MinuteMaidActivity với ngưỡng cao 0.88
                if (errTemplate != null && !string.IsNullOrWhiteSpace(focus) && focus.Contains("MinuteMaidActivity"))
                {
                    using (Bitmap main = SafeScreenShoot(deviceID))
                    {
                        if (main != null)
                        {
                            Point? pt = NeoX.ImageScanOpenCV.FindOutPoint(main, errTemplate, 0.88);
                            if (pt.HasValue) return true;
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        private async Task CloseGoogleErrorAsync(string deviceID, CancellationToken token)
        {
            UpdateProcess(deviceID + " ⚠️ Thấy màn hình 'Couldn't sign in' -> Đóng màn hình lỗi...");
            Android.GuiKey(deviceID, ADBKey.KEYCODE_BACK);
            await DelayWithPause(800, token);
            string curFocus = AdbShell(deviceID, "dumpsys window | grep mCurrentFocus");
            if (curFocus.Contains("ErrorActivity"))
            {
                Android.GuiKey(deviceID, ADBKey.KEYCODE_BACK);
                await DelayWithPause(500, token);
            }
            curFocus = AdbShell(deviceID, "dumpsys window | grep mCurrentFocus");
            if (curFocus.Contains("ErrorActivity"))
            {
                // Tuyệt đối không gọi 'am force-stop com.google.android.gms' để bảo toàn VPNService và kết nối ExpressVPN!
                GoHome(deviceID);
                await DelayWithPause(800, token);
            }
        }

        private async Task HandleGoogleErrorAndChangeDeviceAsync(string deviceID, string acc, CancellationToken token)
        {
            try
            {
                UpdateProcess(deviceID + $" ⚠️ Thấy màn hình 'Couldn't sign in' -> Lưu email ({acc}) & gọi Change Device...");

                // 1. Lưu lại email vào file mail_loi.txt để bảo toàn tài khoản
                try
                {
                    File.AppendAllText("mail_loi.txt", $"{acc}{Environment.NewLine}");
                }
                catch (Exception ex)
                {
                    UpdateProcess(deviceID + " ⚠️ Lỗi khi ghi mail_loi.txt: " + ex.Message);
                }

                // 2. Hoàn lại email vào đầu hàng đợi emailQueue để khi máy khởi động lại xong sẽ chạy lại chính email này từ đầu
                lock (emailQueueLock)
                {
                    try
                    {
                        var list = new List<string>(emailQueue);
                        list.Insert(0, acc);
                        emailQueue.Clear();
                        foreach (var item in list)
                        {
                            emailQueue.Enqueue(item);
                        }

                        string remainingText = string.Join(Environment.NewLine, emailQueue);
                        if (txtEmails.InvokeRequired)
                        {
                            txtEmails.BeginInvoke(new Action(() => txtEmails.Text = remainingText));
                        }
                        else
                        {
                            txtEmails.Text = remainingText;
                        }

                        if (!string.IsNullOrEmpty(selectedEmailFilePath) && File.Exists(selectedEmailFilePath))
                        {
                            File.WriteAllLines(selectedEmailFilePath, emailQueue);
                        }
                    }
                    catch (Exception ex)
                    {
                        UpdateProcess(deviceID + " ⚠️ Lỗi khi hoàn lại email vào hàng đợi: " + ex.Message);
                    }
                }

                // 3. Đóng màn hình lỗi
                await CloseGoogleErrorAsync(deviceID, token);

                // 4. Gọi Change Device (US, Android 15) và chờ máy boot lên, mở khóa màn hình, kiểm tra mạng Wi-Fi sẵn sàng
                UpdateProcess(deviceID + " 🔄 Đang gọi Change Device...");
                bool changed = await ChangeDeviceAsync(deviceID, token);
                if (changed)
                {
                    UpdateProcess(deviceID + " 🟢 Thiết bị đã Change Device thành công & mạng sẵn sàng -> Chạy lại từ đầu!");
                }
                else
                {
                    UpdateProcess(deviceID + " ⚠️ Change Device không báo thành công, nhưng sẽ tiếp tục chạy lại từ đầu!");
                }
            }
            catch (Exception ex)
            {
                UpdateProcess(deviceID + " ⚠️ Lỗi trong HandleGoogleErrorAndChangeDeviceAsync: " + ex.Message);
            }
        }

        private void CaptureErrorSnapshot(string deviceID, string stepName)
        {
            // Đã loại bỏ chức năng lưu error log / ảnh chụp màn hình theo yêu cầu người dùng để tiết kiệm dung lượng
        }

        private bool DismissSystemPopups(string deviceID)
        {
            try
            {
                string xml = Android.GetUIDumpSafe(deviceID);
                if (string.IsNullOrWhiteSpace(xml)) return false;

                // 1. Kiểm tra lỗi ANR hoặc crash ứng dụng hệ thống
                if (xml.Contains("isn't responding") || xml.Contains("không phản hồi") ||
                    xml.Contains("keeps stopping") || xml.Contains("liên tục dừng") ||
                    xml.Contains("has stopped") || xml.Contains("đã dừng"))
                {
                    UpdateProcess(deviceID + " ⚠️ Phát hiện popup thông báo lỗi hệ thống -> Tự động đóng...");
                    if (ifm.ClickByText(deviceID, "Close app") || ClickByDumpXml(deviceID, "Close app", 1)) return true;
                    if (ifm.ClickByText(deviceID, "Đóng ứng dụng") || ClickByDumpXml(deviceID, "Đóng ứng dụng", 1)) return true;
                    if (ifm.ClickByText(deviceID, "Wait") || ClickByDumpXml(deviceID, "Wait", 1)) return true;
                    if (ifm.ClickByText(deviceID, "OK") || ClickByDumpXml(deviceID, "OK", 1)) return true;
                }

                // 2. Popup cấp quyền hệ thống bất ngờ
                if (xml.Contains("permission_allow_button") || xml.Contains("grant_dialog") || xml.Contains("Allow") || xml.Contains("Cho phép"))
                {
                    if (ClickByResourceId(deviceID, "permission_allow_button", xml)) return true;
                    if (ifm.ClickByText(deviceID, "While using the app") || ClickByDumpXml(deviceID, "While using the app", 1)) return true;
                    if (ifm.ClickByText(deviceID, "Allow") || ClickByDumpXml(deviceID, "Allow", 1)) return true;
                }
            }
            catch { }
            return false;
        }

        private bool IsCurrentFocus(string deviceID, string pkgOrAct)
        {
            try
            {
                string focus = AdbShell(deviceID, "dumpsys window | grep -E 'mCurrentFocus|mFocusedApp'");
                return !string.IsNullOrWhiteSpace(focus) && focus.Contains(pkgOrAct);
            }
            catch { return false; }
        }

        private bool FastClickGoogleTermsButton(string deviceID, ImgClone img, bool canUnderstand, bool canAgree, bool canMore, bool canAccept, out string clickedButton)
        {
            clickedButton = "";
            try
            {
                // 1. DUMP XML 1 LẦN DUY NHẤT để kiểm tra cực nhanh toàn bộ các nút
                string xml = Android.GetUIDumpSafe(deviceID);
                if (!string.IsNullOrWhiteSpace(xml))
                {
                    string lowerXml = xml.ToLower();

                    // ƯU TIÊN 1: Nút Accept / Chấp nhận (nếu được phép)
                    if (canAccept && (lowerXml.Contains("accept") || lowerXml.Contains("chấp nhận")))
                    {
                        if (ClickByCaseInsensitiveText(deviceID, xml, "Accept") ||
                            ClickByCaseInsensitiveText(deviceID, xml, "Chấp nhận") ||
                            ClickByDumpXml(deviceID, "Accept", 1, 100))
                        {
                            clickedButton = "accept";
                            return true;
                        }
                    }

                    // ƯU TIÊN 2: Nút More / Thêm (nếu được phép)
                    if (canMore && (lowerXml.Contains("more") || lowerXml.Contains("thêm")))
                    {
                        if (ClickByCaseInsensitiveText(deviceID, xml, "More") ||
                            ClickByCaseInsensitiveText(deviceID, xml, "Thêm") ||
                            ClickByDumpXml(deviceID, "More", 1, 100))
                        {
                            clickedButton = "more";
                            return true;
                        }
                    }

                    // ƯU TIÊN 3: Nút I agree / Tôi đồng ý (nếu được phép)
                    if (canAgree && (lowerXml.Contains("agree") || lowerXml.Contains("đồng ý")))
                    {
                        if (ClickByCaseInsensitiveText(deviceID, xml, "I agree") ||
                            ClickByCaseInsensitiveText(deviceID, xml, "Tôi đồng ý") ||
                            ClickByCaseInsensitiveText(deviceID, xml, "Agree") ||
                            ClickByDumpXml(deviceID, "I agree", 1, 100))
                        {
                            clickedButton = "agree";
                            return true;
                        }
                    }

                    // ƯU TIÊN 4: Nút I understand / Tôi hiểu (nếu được phép)
                    if (canUnderstand && (lowerXml.Contains("understand") || lowerXml.Contains("hiểu")))
                    {
                        if (ClickByCaseInsensitiveText(deviceID, xml, "I understand") ||
                            ClickByCaseInsensitiveText(deviceID, xml, "Tôi hiểu") ||
                            ClickByCaseInsensitiveText(deviceID, xml, "Understand") ||
                            ClickByDumpXml(deviceID, "I understand", 1, 100))
                        {
                            clickedButton = "understand";
                            return true;
                        }
                    }
                }

                // 2. CHỤP 1 ẢNH DUY NHẤT để quét OpenCV / Pixel (nếu XML không tìm thấy node)
                if (canAccept || canAgree || canUnderstand)
                {
                    using (Bitmap screen = SafeScreenShoot(deviceID))
                    {
                        if (screen != null)
                        {
                            // 2.1 Kiểm tra ảnh mẫu Accept (tính tâm để bấm)
                            if (canAccept && img.accept != null)
                            {
                                Point? ptAccept = NeoX.ImageScanOpenCV.FindOutPoint(screen, img.accept, 0.72);
                                if (ptAccept.HasValue)
                                {
                                    int cx = ptAccept.Value.X + img.accept.Width / 2;
                                    int cy = ptAccept.Value.Y + img.accept.Height / 2;
                                    UpdateProcess(deviceID + $" 👉 Tìm thấy ảnh template 'Accept' tại ({cx}, {cy}) -> Bấm!");
                                    Android.Tap(deviceID, cx, cy);
                                    clickedButton = "accept";
                                    return true;
                                }
                            }

                            // 2.2 Kiểm tra ảnh mẫu I agree (pill) (tính tâm để bấm)
                            if (canAgree && img.iage_new != null)
                            {
                                Point? pt = NeoX.ImageScanOpenCV.FindOutPoint(screen, img.iage_new, 0.70);
                                if (pt.HasValue)
                                {
                                    int cx = pt.Value.X + img.iage_new.Width / 2;
                                    int cy = pt.Value.Y + img.iage_new.Height / 2;
                                    UpdateProcess(deviceID + $" 👉 Tìm thấy ảnh template 'I agree' (pill) tại ({cx}, {cy}) -> Bấm!");
                                    Android.Tap(deviceID, cx, cy);
                                    clickedButton = "agree";
                                    return true;
                                }
                            }

                            // 2.3 Kiểm tra ảnh mẫu I agree (classic) (tính tâm để bấm)
                            if (canAgree && img.iage != null)
                            {
                                Point? pt = NeoX.ImageScanOpenCV.FindOutPoint(screen, img.iage, 0.70);
                                if (pt.HasValue)
                                {
                                    int cx = pt.Value.X + img.iage.Width / 2;
                                    int cy = pt.Value.Y + img.iage.Height / 2;
                                    UpdateProcess(deviceID + $" 👉 Tìm thấy ảnh template 'I agree' (classic) tại ({cx}, {cy}) -> Bấm!");
                                    Android.Tap(deviceID, cx, cy);
                                    clickedButton = "agree";
                                    return true;
                                }
                            }

                            // 2.4 Kiểm tra ảnh mẫu I understand (ngưỡng 0.75 tránh match nhầm I agree, tính tâm để bấm)
                            if (canUnderstand && img.i_understand != null)
                            {
                                Point? pt = NeoX.ImageScanOpenCV.FindOutPoint(screen, img.i_understand, 0.75);
                                if (pt.HasValue)
                                {
                                    int cx = pt.Value.X + img.i_understand.Width / 2;
                                    int cy = pt.Value.Y + img.i_understand.Height / 2;
                                    UpdateProcess(deviceID + $" 👉 Tìm thấy ảnh template 'I understand' tại ({cx}, {cy}) -> Bấm!");
                                    Android.Tap(deviceID, cx, cy);
                                    clickedButton = "understand";
                                    return true;
                                }
                            }

                            // 2.5 Quét pixel nút xanh Material 3 ở góc dưới cùng bên phải
                            if (canAgree || canUnderstand)
                            {
                                int w = screen.Width;
                                int h = screen.Height;
                                int startX = (int)(w * 0.50);
                                int endX = (int)(w * 0.98);
                                int startY = (int)(h * 0.85);
                                int endY = (int)(h * 0.98);

                                int minX = w, maxX = 0, minY = h, maxY = 0;
                                int bluePixelCount = 0;

                                for (int y = startY; y < endY; y += 3)
                                {
                                    for (int x = startX; x < endX; x += 3)
                                    {
                                        Color c = screen.GetPixel(x, y);
                                        if (c.B > 190 && c.G > 150 && c.R > 120 && (c.B > c.R + 25))
                                        {
                                            bluePixelCount++;
                                            if (x < minX) minX = x;
                                            if (x > maxX) maxX = x;
                                            if (y < minY) minY = y;
                                            if (y > maxY) maxY = y;
                                        }
                                    }
                                }

                                if (bluePixelCount >= 200 && maxX > minX && maxY > minY)
                                {
                                    int btnW = maxX - minX;
                                    int centerX = (minX + maxX) / 2;
                                    int centerY = (minY + maxY) / 2;
                                    int widthThreshold = (int)(390.0 * w / 1440.0);

                                    if (canAgree && !canUnderstand)
                                    {
                                        clickedButton = "agree";
                                        UpdateProcess(deviceID + $" 👉 Phát hiện nút 'I agree' qua màu xanh ({btnW}px) -> Bấm ({centerX}, {centerY})!");
                                        Android.Tap(deviceID, centerX, centerY);
                                        return true;
                                    }
                                    else if (!canAgree && canUnderstand)
                                    {
                                        clickedButton = "understand";
                                        UpdateProcess(deviceID + $" 👉 Phát hiện nút 'I understand' qua màu xanh ({btnW}px) -> Bấm ({centerX}, {centerY})!");
                                        Android.Tap(deviceID, centerX, centerY);
                                        return true;
                                    }
                                    else if (canAgree && canUnderstand)
                                    {
                                        if (btnW < widthThreshold)
                                        {
                                            clickedButton = "agree";
                                            UpdateProcess(deviceID + $" 👉 Phát hiện nút 'I agree' qua màu xanh ({btnW}px < {widthThreshold}px) -> Bấm ({centerX}, {centerY})!");
                                            Android.Tap(deviceID, centerX, centerY);
                                            return true;
                                        }
                                        else
                                        {
                                            clickedButton = "understand";
                                            UpdateProcess(deviceID + $" 👉 Phát hiện nút 'I understand' qua màu xanh ({btnW}px >= {widthThreshold}px) -> Bấm ({centerX}, {centerY})!");
                                            Android.Tap(deviceID, centerX, centerY);
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateProcess(deviceID + " ⚠️ Lỗi trong FastClickGoogleTermsButton: " + ex.Message);
            }

            return false;
        }

        private bool CheckAndClickUnderstandOrAgree(string deviceID, ImgClone img, CancellationToken token, out string clickedType)
        {
            return FastClickGoogleTermsButton(deviceID, img, true, true, false, false, out clickedType);
        }

        private bool IsGooglePasswordError(string deviceID, Bitmap tPwError = null)
        {
            try
            {
                // 1. Kiểm tra nhanh qua XML dump nếu có thông báo sai mật khẩu THẬT SỰ
                string xml = Android.GetUIDumpSafe(deviceID);
                if (!string.IsNullOrWhiteSpace(xml))
                {
                    // Chỉ bắt các câu thông báo sai mật khẩu thực sự, TUYỆT ĐỐI không bắt placeholder "Enter a password" hay "Nhập mật khẩu"
                    if (xml.Contains("Wrong password") ||
                        xml.Contains("Sai mật khẩu") ||
                        xml.Contains("Mật khẩu không chính xác") ||
                        xml.Contains("Incorrect password") ||
                        xml.Contains("Mật khẩu sai"))
                    {
                        return true;
                    }
                }

                // 2. Chụp màn hình để kiểm tra pixel màu đỏ/hồng error hoặc template icon (!)
                using (Bitmap screen = SafeScreenShoot(deviceID))
                {
                    if (screen != null)
                    {
                        // 2.1 Kiểm tra qua template icon (!) nếu có (ngưỡng cao 0.80 tránh match nhầm)
                        if (tPwError != null)
                        {
                            Point? pt = NeoX.ImageScanOpenCV.FindOutPoint(screen, tPwError, 0.80);
                            if (pt.HasValue) return true;
                        }

                        // 2.2 Quét pixel màu đỏ/hồng error (#f2b8b5 / #d93025) ở vùng thông báo lỗi dưới ô mật khẩu (y: 35%..55%)
                        int w = screen.Width;
                        int h = screen.Height;
                        int startY = (int)(h * 0.35);
                        int endY = (int)(h * 0.55);
                        int startX = (int)(w * 0.08);
                        int endX = (int)(w * 0.92);

                        int errorPixelCount = 0;
                        for (int y = startY; y < endY; y += 3)
                        {
                            for (int x = startX; x < endX; x += 3)
                            {
                                Color c = screen.GetPixel(x, y);
                                // Google Dark Theme error (#f2b8b5): R ~ 242, G ~ 184, B ~ 181
                                bool isDarkError = (c.R > 220 && c.G > 150 && c.G < 210 && c.B > 150 && c.B < 210 && (c.R > c.G + 35) && (c.R > c.B + 35));
                                // Google Light Theme error (#d93025 / #b3261e): R > 180, G < 90, B < 90
                                bool isLightError = (c.R > 180 && c.G < 90 && c.B < 90);

                                if (isDarkError || isLightError)
                                {
                                    errorPixelCount++;
                                    if (errorPixelCount > 200) return true;
                                }
                            }
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        private bool IsGooglePasswordScreen(string deviceID, Bitmap tShow, Bitmap tWelcome)
        {
            try
            {
                // 1. Kiểm tra nhanh qua XML dump
                string xml = Android.GetUIDumpSafe(deviceID);
                if (!string.IsNullOrWhiteSpace(xml))
                {
                    // Nếu đã có text I agree hoặc I understand -> RÕ RÀNG KHÔNG CÒN LÀ MÀN HÌNH PASSWORD NỮA!
                    if (xml.Contains("I agree") || xml.Contains("I understand"))
                    {
                        return false;
                    }

                    // Chỉ coi là màn hình Password khi có các nút/nhãn đặc trưng của ô nhập password
                    if (xml.Contains("Enter your password") || xml.Contains("Show password") ||
                        xml.Contains("Nhập mật khẩu") || xml.Contains("Hiện mật khẩu") ||
                        xml.Contains("Quên mật khẩu") || xml.Contains("Forgot password"))
                    {
                        return true;
                    }
                }

                // 2. Kiểm tra qua hình ảnh template (chỉ khi không có text I agree / I understand)
                if (string.IsNullOrWhiteSpace(xml) || (!xml.Contains("I agree") && !xml.Contains("I understand")))
                {
                    using (Bitmap main = SafeScreenShoot(deviceID))
                    {
                        if (main == null) return false;

                        if (tShow != null)
                        {
                            Point? pt = NeoX.ImageScanOpenCV.FindOutPoint(main, tShow, 0.75);
                            if (pt.HasValue) return true;
                        }

                        if (tWelcome != null)
                        {
                            Point? pt = NeoX.ImageScanOpenCV.FindOutPoint(main, tWelcome, 0.75);
                            if (pt.HasValue) return true;
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        private int SafeTimAnh(string deviceID, int loop, CancellationToken token, params Bitmap[] templates)
        {
            for (int i = 0; i < loop; i++)
            {
                if (token.IsCancellationRequested) return -1;
                CheckPause(token);

                using (Bitmap mainBitmap = SafeScreenShoot(deviceID))
                {
                    if (mainBitmap != null)
                    {
                        for (int j = 0; j < templates.Length; j++)
                        {
                            Bitmap bitmap = templates[j];
                            if (bitmap != null)
                            {
                                Point? point = NeoX.ImageScanOpenCV.FindOutPoint(mainBitmap, bitmap, 0.8);
                                if (point.HasValue)
                                {
                                    return j;
                                }
                            }
                        }
                    }
                }

                try
                {
                    Task.Delay(1000, token).Wait(token);
                }
                catch
                {
                    return -1;
                }
            }
            return -1;
        }

        private int SafeTimAnh(string deviceID, int loop, params Bitmap[] templates)
        {
            return SafeTimAnh(deviceID, loop, CancellationToken.None, templates);
        }

        private bool SafeClickVaoAnh(string deviceID, CancellationToken token, params Bitmap[] templates)
        {
            for (int attempt = 0; attempt < 8 && !token.IsCancellationRequested; attempt++)
            {
                CheckPause(token);

                using (Bitmap mainBitmap = SafeScreenShoot(deviceID))
                {
                    if (mainBitmap != null)
                    {
                        foreach (Bitmap bitmap in templates)
                        {
                            if (bitmap != null)
                            {
                                Point? point = NeoX.ImageScanOpenCV.FindOutPoint(mainBitmap, bitmap, 0.8);
                                if (point.HasValue)
                                {
                                    Android.Tap(deviceID, point.Value.X, point.Value.Y);
                                    return true;
                                }
                            }
                        }
                    }
                }

                try
                {
                    Task.Delay(600, token).Wait(token);
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        private bool SafeClickVaoAnh(string deviceID, params Bitmap[] templates)
        {
            return SafeClickVaoAnh(deviceID, CancellationToken.None, templates);
        }

        /// <summary>
        /// Bấm nút chính của TikTok (Next / Continue / Confirm...) hỗ trợ cả nút màu ĐỎ và nút màu ĐEN.
        /// Tự động tìm qua:
        /// 1. UI Dump XML (text: Next, Continue, Confirm, Xác nhận... hoặc resource-id: amb, continue_button, fmw... hoặc android.widget.Button)
        /// 2. Quét Pixel màn hình: Nhận diện dải nút màu ĐỎ (#FE2C55) hoặc màu ĐEN (#000000 / đen đậm)
        /// 3. ifm.ClickByText fallback
        /// 4. Tọa độ fallback
        /// </summary>
        private bool ClickTikTokPrimaryButton(string deviceID, string buttonContext = "Next", double startYRatio = 0.28, double endYRatio = 0.45, int fallbackX = 720, int fallbackY = 1040, Bitmap templateBmp = null)
        {
            try
            {
                // 1. Tìm qua UI Dump XML
                string xml = Android.GetUIDumpSafe(deviceID);
                if (!string.IsNullOrWhiteSpace(xml))
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(xml);

                        // 1.1. Tìm theo resource-id chuẩn của button TikTok
                        // LƯU Ý: Tuyệt đối tránh 'am6' vì 'am6' là EditText ngày sinh
                        var resNode = doc.SelectSingleNode("//node[(@resource-id='com.zhiliaoapp.musically:id/amb' or contains(@resource-id, ':id/amb') or contains(@resource-id, 'continue_button') or contains(@resource-id, ':id/fmw')) and not(contains(@resource-id, 'am6'))]");
                        if (resNode != null && GetCenter(resNode.Attributes?["bounds"]?.Value, out int rx, out int ry))
                        {
                            UpdateProcess(deviceID + $" 👉 Click TikTok ({buttonContext}) qua resource-id tại ({rx}, {ry})");
                            KAutoHelper.ADBHelper.Tap(deviceID, rx, ry);
                            return true;
                        }

                        // 1.2. Tìm theo text / content-desc của button
                        string[] targetTexts = new string[] { "Next", "Continue", "Confirm", "Confirm nickname", "Tiếp tục", "Tiếp theo", "Xác nhận", "Done", "Sign up" };
                        foreach (var txt in targetTexts)
                        {
                            var txtNode = doc.SelectSingleNode($"//node[(@class='android.widget.Button' or @clickable='true' or @focusable='true') and (@text='{txt}' or @content-desc='{txt}')]");
                            if (txtNode != null && GetCenter(txtNode.Attributes?["bounds"]?.Value, out int tx, out int ty))
                            {
                                UpdateProcess(deviceID + $" 👉 Click TikTok ({buttonContext}) qua text '{txt}' tại ({tx}, {ty})");
                                KAutoHelper.ADBHelper.Tap(deviceID, tx, ty);
                                return true;
                            }
                        }

                        // 1.3. Tìm android.widget.Button trong vùng Y mong muốn (startYRatio..endYRatio)
                        var buttonNodes = doc.SelectNodes("//node[@class='android.widget.Button' and (@clickable='true' or not(@clickable='false'))]");
                        if (buttonNodes != null)
                        {
                            foreach (XmlNode bNode in buttonNodes)
                            {
                                string rId = bNode.Attributes?["resource-id"]?.Value ?? "";
                                if (rId.Contains("am6")) continue;

                                if (GetCenter(bNode.Attributes?["bounds"]?.Value, out int bx, out int by))
                                {
                                    int targetMinY = (int)(2960 * startYRatio);
                                    int targetMaxY = (int)(2960 * endYRatio);
                                    if (by >= targetMinY && by <= targetMaxY)
                                    {
                                        UpdateProcess(deviceID + $" 👉 Click TikTok ({buttonContext}) qua class Button tại ({bx}, {by})");
                                        KAutoHelper.ADBHelper.Tap(deviceID, bx, by);
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                }

                // 2. Quét Pixel màn hình: Nhận diện dải nút màu ĐỎ hoặc ĐEN
                // (Theo yêu cầu: hỗ trợ trường hợp nút hiện màu đen thay vì màu đỏ)
                using (Bitmap screen = SafeScreenShoot(deviceID))
                {
                    if (screen != null)
                    {
                        // 2.1. OpenCV template match với tieptuc nếu có
                        if (templateBmp != null)
                        {
                            try
                            {
                                Point? pt = NeoX.ImageScanOpenCV.FindOutPoint(screen, templateBmp, 0.70);
                                if (pt.HasValue)
                                {
                                    int cx = pt.Value.X + templateBmp.Width / 2;
                                    int cy = pt.Value.Y + templateBmp.Height / 2;
                                    UpdateProcess(deviceID + $" 🔴 Click nút TikTok ({buttonContext}) qua template tại ({cx}, {cy})");
                                    KAutoHelper.ADBHelper.Tap(deviceID, cx, cy);
                                    return true;
                                }
                            }
                            catch { }
                        }

                        // 2.2. Quét pixel trực tiếp cho dải nút ĐỎ (#FE2C55) và dải nút ĐEN (#000000)
                        int w = screen.Width;
                        int h = screen.Height;
                        int startY = (int)(h * startYRatio);
                        int endY = (int)(h * endYRatio);

                        int redTopY = -1, redBottomY = -1;
                        int redMinX = w, redMaxX = 0;

                        int blackTopY = -1, blackBottomY = -1;
                        int blackMinX = w, blackMaxX = 0;

                        // Quét từng hàng với bước 4px
                        for (int y = startY; y < endY; y += 4)
                        {
                            int redCount = 0;
                            int rRowMinX = w, rRowMaxX = 0;

                            int blackCount = 0;
                            int bRowMinX = w, bRowMaxX = 0;

                            for (int x = (int)(w * 0.08); x < (int)(w * 0.92); x += 4)
                            {
                                Color c = screen.GetPixel(x, y);

                                // Nút màu ĐỎ TikTok (#FE2C55 / RGB: R > 200, G < 80, B < 100)
                                if (c.R > 200 && c.G < 80 && c.B < 100)
                                {
                                    redCount++;
                                    if (x < rRowMinX) rRowMinX = x;
                                    if (x > rRowMaxX) rRowMaxX = x;
                                }
                                // Nút màu ĐEN TikTok (#000000 / RGB: R < 35, G < 35, B < 35)
                                else if (c.R < 35 && c.G < 35 && c.B < 35)
                                {
                                    blackCount++;
                                    if (x < bRowMinX) bRowMinX = x;
                                    if (x > bRowMaxX) bRowMaxX = x;
                                }
                            }

                            // Hàng chứa nút ĐỎ: chiều ngang rộng > 45% màn hình, có ít nhất 60 mẫu đỏ
                            if (redCount >= 60 && (rRowMaxX - rRowMinX) > w * 0.45)
                            {
                                if (redTopY == -1) redTopY = y;
                                redBottomY = y;
                                if (rRowMinX < redMinX) redMinX = rRowMinX;
                                if (rRowMaxX > redMaxX) redMaxX = rRowMaxX;
                            }

                            // Hàng chứa nút ĐEN: chiều ngang rộng > 45% màn hình, có ít nhất 80 mẫu đen
                            if (blackCount >= 80 && (bRowMaxX - bRowMinX) > w * 0.45)
                            {
                                if (blackTopY == -1) blackTopY = y;
                                blackBottomY = y;
                                if (bRowMinX < blackMinX) blackMinX = bRowMinX;
                                if (bRowMaxX > blackMaxX) blackMaxX = bRowMaxX;
                            }
                        }

                        // Nếu phát hiện dải nút ĐỎ có chiều cao >= 40px
                        if (redTopY != -1 && (redBottomY - redTopY) >= 40)
                        {
                            int cx = (redMinX + redMaxX) / 2;
                            int cy = (redTopY + redBottomY) / 2;
                            UpdateProcess(deviceID + $" 🔴 Phát hiện nút TikTok màu ĐỎ ({buttonContext}) tại ({cx}, {cy}) -> Tap!");
                            KAutoHelper.ADBHelper.Tap(deviceID, cx, cy);
                            return true;
                        }

                        // Nếu phát hiện dải nút ĐEN có chiều cao >= 40px
                        if (blackTopY != -1 && (blackBottomY - blackTopY) >= 40)
                        {
                            int cx = (blackMinX + blackMaxX) / 2;
                            int cy = (blackTopY + blackBottomY) / 2;
                            UpdateProcess(deviceID + $" ⚫ Phát hiện nút TikTok màu ĐEN ({buttonContext}) tại ({cx}, {cy}) -> Tap!");
                            KAutoHelper.ADBHelper.Tap(deviceID, cx, cy);
                            return true;
                        }
                    }
                }

                // 3. Fallback qua ifm.ClickByText
                if (ifm.ClickByText(deviceID, "Next") || ifm.ClickByText(deviceID, "Continue") ||
                    ifm.ClickByText(deviceID, "Confirm") || ifm.ClickByText(deviceID, "Tiếp tục"))
                {
                    UpdateProcess(deviceID + $" 👉 Click text nút TikTok ({buttonContext}) qua ifm.ClickByText");
                    return true;
                }

                // 4. Fallback qua tọa độ
                if (IsCurrentFocus(deviceID, "com.zhiliaoapp.musically"))
                {
                    UpdateProcess(deviceID + $" 👉 Tap tọa độ fallback ({fallbackX}, {fallbackY}) cho nút TikTok ({buttonContext})...");
                    KAutoHelper.ADBHelper.Tap(deviceID, fallbackX, fallbackY);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ClickTikTokPrimaryButton error: {ex.Message}");
            }
            return false;
        }



        string SelectedVpnProvider
        {
            get
            {
                if (cboVpnProvider == null) return "ExpressVPN";
                if (cboVpnProvider.InvokeRequired)
                {
                    return (string)cboVpnProvider.Invoke(new Func<string>(() => cboVpnProvider.SelectedItem?.ToString() ?? "ExpressVPN"));
                }
                return cboVpnProvider.SelectedItem?.ToString() ?? "ExpressVPN";
            }
        }

        private void cboVpnProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = SelectedVpnProvider;
            try
            {
                File.WriteAllText("vpn_provider.txt", selected);
            }
            catch { }

            bool isExpress = selected == "ExpressVPN";
            txtExpressVpn.Enabled = isExpress;
            if (isExpress)
            {
                labelExpressVpn.Text = "🔒 ExpressVPN (email|pass):";
                txtExpressVpn.BackColor = Color.White;
            }
            else
            {
                labelExpressVpn.Text = "🔒 ExpressVPN (Không cần với HMA):";
                txtExpressVpn.BackColor = Color.FromArgb(241, 245, 249);
            }
        }

        string VpnAccountText
        {
            get
            {
                if (txtExpressVpn.InvokeRequired)
                {
                    return (string)txtExpressVpn.Invoke(new Func<string>(() => txtExpressVpn.Text));
                }
                return txtExpressVpn.Text;
            }
        }

        async Task HandleVpnPermissionsAsync(string deviceID, CancellationToken token)
        {
            for (int p = 0; p < 8; p++)
            {
                string xml = Android.GetUIDumpSafe(deviceID);
                if (string.IsNullOrWhiteSpace(xml))
                {
                    await DelayWithPause(1500, token);
                    continue;
                }

                // 0. Nếu xuất hiện popup "That's it, you're protected!" thì bấm Close để vào màn hình chính
                if (xml.Contains("you’re protected") || xml.Contains("you're protected") || xml.Contains("ProtectionSummaryBump") || (xml.Contains("Selected Location") && xml.Contains("Close")))
                {
                    await DismissProtectionBumpAsync(deviceID, token);
                    break;
                }

                // 0.1 Nếu đã ở màn hình chính VPN và KHÔNG có dialog cấp quyền -> Thoát ngay!
                bool isMainScreen = xml.Contains("vpn_location_picker_button") || xml.Contains("Fastest Location") || xml.Contains("Selected Location") || xml.Contains("Change") || xml.Contains("Protected") || xml.Contains("Not protected") || xml.Contains("Not connected");
                bool hasPermissionDialog = xml.Contains("Set Up Your VPN") || xml.Contains("onboarding_vpn_configuration") || xml.Contains("permission to complete the configuration") ||
                                           xml.Contains("Connection request") || xml.Contains("com.android.vpndialogs") || xml.Contains("wants to set up a VPN connection") ||
                                           xml.Contains("permission_allow_button") || xml.Contains("Allow ExpressVPN");

                if (isMainScreen && !hasPermissionDialog)
                {
                    break;
                }

                // Nếu có thông báo sai tài khoản / mật khẩu, thoát ngay để SetupExpressVpnAsync xử lý restart app
                if (xml.Contains("Unable to Sign In") || xml.Contains("make sure your email address and password are correct"))
                {
                    break;
                }

                // Popup bottom-sheet ExpressKeys khi đăng nhập thành công (chỉ xử lý khi CÓ nút Skip / Close sheet và CHƯA vào màn hình chính)
                if (!isMainScreen && (xml.Contains("Skip") || xml.Contains("Close sheet")) && (xml.Contains("Unlock the New ExpressKeys App") || xml.Contains("ExpressKeys")))
                {
                    UpdateProcess(deviceID + " 👉 Bấm Skip tại popup ExpressKeys...");
                    if (!ClickByDumpXml(deviceID, "Skip", 2, 800))
                    {
                        if (!ClickByDumpXml(deviceID, "Close sheet", 1, 500))
                        {
                            KAutoHelper.ADBHelper.Tap(deviceID, 720, 2708);
                        }
                    }
                    await DelayWithPause(2000, token);
                    continue;
                }

                // 1. Màn hình "Set Up Your VPN" trong app ExpressVPN
                if (xml.Contains("Set Up Your VPN") || xml.Contains("onboarding_vpn_configuration") || xml.Contains("permission to complete the configuration"))
                {
                    UpdateProcess(deviceID + " 🟢 Bấm OK tại màn hình Set Up Your VPN...");
                    if (!ClickByResourceId(deviceID, "onboarding_vpn_configuration_ok_button"))
                    {
                        if (!ifm.ClickByText(deviceID, "OK"))
                        {
                            KAutoHelper.ADBHelper.Tap(deviceID, 720, 2428);
                        }
                    }
                    await DelayWithPause(2000, token);
                    continue;
                }

                // 2. Popup hệ thống Android "Connection request"
                if (xml.Contains("Connection request") || xml.Contains("com.android.vpndialogs") || xml.Contains("wants to set up a VPN connection") || xml.Contains("Yêu cầu kết nối"))
                {
                    UpdateProcess(deviceID + " 🟢 Bấm OK tại popup Connection request hệ thống...");
                    ClickHmaConnectionRequestOk(deviceID, xml);
                    await DelayWithPause(2500, token);
                    continue;
                }

                // 3. Quyền thông báo (Android 13+)
                if (xml.Contains("permission_allow_button") || xml.Contains("Allow ExpressVPN"))
                {
                    UpdateProcess(deviceID + " 🟢 Cấp quyền thông báo (Allow)...");
                    if (!ClickByResourceId(deviceID, "com.android.permissioncontroller:id/permission_allow_button"))
                    {
                        ifm.ClickByText(deviceID, "Allow");
                    }
                    await DelayWithPause(1500, token);
                    continue;
                }

                if (xml.Contains("No Thanks"))
                {
                    ifm.ClickByText(deviceID, "No Thanks");
                    await DelayWithPause(1500, token);
                    continue;
                }

                if (xml.Contains("Continue"))
                {
                    ifm.ClickByText(deviceID, "Continue");
                    await DelayWithPause(1500, token);
                    continue;
                }

                // Nếu đã ở màn hình chính VPN
                if (xml.Contains("vpn_location_picker_button") || xml.Contains("Selected Location") || xml.Contains("Not connected") || xml.Contains("Connecting"))
                {
                    break;
                }

                await DelayWithPause(1500, token);
            }
        }

        async Task<bool> DismissProtectionBumpAsync(string deviceID, CancellationToken token)
        {
            for (int i = 0; i < 5; i++)
            {
                if (token.IsCancellationRequested) return false;

                string xml = Android.GetUIDumpSafe(deviceID);
                if (string.IsNullOrWhiteSpace(xml))
                {
                    await DelayWithPause(1000, token);
                    continue;
                }

                // Kiểm tra xem có popup "That’s it, you’re protected!" không
                bool hasBump = xml.Contains("you’re protected") || xml.Contains("you're protected") ||
                               xml.Contains("ProtectionSummaryBump") || xml.Contains("Close sheet") ||
                               (xml.Contains("Your internet is now private") && xml.Contains("Close")) ||
                               (xml.Contains("IP Address") && xml.Contains("Close"));

                if (hasBump)
                {
                    UpdateProcess(deviceID + " 👉 Bấm Close tại popup 'That’s it, you’re protected!' để vào màn hình chính...");
                    if (!ClickByDumpXml(deviceID, "Close", 1, 500))
                    {
                        if (!ClickByDumpXml(deviceID, "Close sheet", 1, 500))
                        {
                            KAutoHelper.ADBHelper.Tap(deviceID, 720, 2614);
                        }
                    }
                    await DelayWithPause(1500, token);
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        async Task<bool> SetupExpressVpnAsync(string deviceID, string vpnAccount, CancellationToken token)
        {
            UpdateProcess(deviceID + " 🛡️ Bắt đầu thiết lập ExpressVPN...");

            // 1. Mở app ExpressVPN
            AdbShell(deviceID, "monkey -p com.expressvpn.vpn -c android.intent.category.LAUNCHER 1");
            await DelayWithPause(1500, token);

            // 2. Kiểm tra giao diện ExpressVPN
            for (int attempt = 0; attempt < 25; attempt++)
            {
                if (token.IsCancellationRequested) return false;

                string xml = Android.GetUIDumpSafe(deviceID);
                if (string.IsNullOrWhiteSpace(xml))
                {
                    await DelayWithPause(1500, token);
                    continue;
                }

                // Nếu có thông báo sai mật khẩu / không thể đăng nhập
                if (xml.Contains("Unable to Sign In") || xml.Contains("make sure your email address and password are correct"))
                {
                    UpdateProcess(deviceID + " ❌ Báo sai thông tin đăng nhập ExpressVPN (Unable to Sign In)!");
                    UpdateProcess(deviceID + " 🔄 Đóng app và mở lại đăng nhập lại từ đầu...");

                    // Bấm OK nếu có
                    if (!ClickByDumpXml(deviceID, "OK", 1, 500))
                    {
                        KAutoHelper.ADBHelper.Tap(deviceID, 1095, 1633);
                    }
                    await DelayWithPause(1000, token);

                    // Thoát về Home và mở lại app ExpressVPN (không force-stop để tránh mất cấu hình)
                    GoHome(deviceID);
                    await DelayWithPause(1500, token);

                    // Mở lại app ExpressVPN
                    AdbShell(deviceID, "monkey -p com.expressvpn.vpn -c android.intent.category.LAUNCHER 1");
                    await DelayWithPause(2000, token);
                    continue;
                }

                // 0. Nếu đang có popup "That's it, you're protected!" thì bấm Close để vào màn hình chính
                if (xml.Contains("you’re protected") || xml.Contains("you're protected") || xml.Contains("ProtectionSummaryBump"))
                {
                    await DismissProtectionBumpAsync(deviceID, token);
                    break;
                }

                // 0.1 Nếu đã ở màn hình chính ExpressVPN -> Hoàn tất đăng nhập, sang bước chọn Location!
                if (xml.Contains("vpn_location_picker_button") || xml.Contains("Fastest Location") || xml.Contains("Selected Location") || xml.Contains("Change") || xml.Contains("Protected") || xml.Contains("Not connected") || xml.Contains("Not protected"))
                {
                    UpdateProcess(deviceID + " 🏠 Đã ở màn hình chính ExpressVPN!");
                    break;
                }

                // 2. Nếu xuất hiện popup bottom-sheet ExpressKeys đẩy lên sau khi đăng nhập thành công
                if ((xml.Contains("Skip") || xml.Contains("Close sheet")) && (xml.Contains("Unlock the New ExpressKeys App") || xml.Contains("ExpressKeys")))
                {
                    UpdateProcess(deviceID + " 👉 Bấm Skip tại popup ExpressKeys để vào màn hình chính...");
                    if (!ClickByDumpXml(deviceID, "Skip", 2, 800))
                    {
                        if (!ClickByDumpXml(deviceID, "Close sheet", 1, 500))
                        {
                            KAutoHelper.ADBHelper.Tap(deviceID, 720, 2708);
                        }
                    }
                    await DelayWithPause(1500, token);

                    // Kiểm tra ngay sau khi bấm Skip
                    string afterSkipXml = Android.GetUIDumpSafe(deviceID);
                    if (!string.IsNullOrWhiteSpace(afterSkipXml) && (afterSkipXml.Contains("vpn_location_picker_button") || afterSkipXml.Contains("Fastest Location") || afterSkipXml.Contains("Change") || afterSkipXml.Contains("Not protected") || afterSkipXml.Contains("Protected") || afterSkipXml.Contains("Not connected")))
                    {
                        UpdateProcess(deviceID + " 🏠 Đã vào màn hình chính ExpressVPN sau khi Skip!");
                        break;
                    }
                    continue;
                }

                // Nếu đang ở màn hình chào mừng (Welcome) có nút Sign In nhưng chưa vào form nhập
                if (!xml.Contains("signin_email_field") && !xml.Contains("signin_password_field") && 
                    (xml.Contains("Sign In") || xml.Contains("Sign in") || xml.Contains("Have an account") || xml.Contains("Get ExpressVPN")))
                {
                    UpdateProcess(deviceID + " 🔑 Bấm Sign In ExpressVPN theo tỉ lệ cố định (X=66%, Y=95%)...");
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 66.0, 95.0);
                    await DelayWithPause(1200, token);

                    // Kiểm tra nếu chưa chuyển màn hình thì thử fallback click bằng XML
                    xml = Android.GetUIDumpSafe(deviceID);
                    if (!xml.Contains("signin_email_field") && !xml.Contains("signin_password_field"))
                    {
                        if (!ClickByDumpXml(deviceID, "Sign in", 1, 300))
                        {
                            ClickByDumpXml(deviceID, "Sign In", 1, 300);
                        }
                        await DelayWithPause(1200, token);
                        xml = Android.GetUIDumpSafe(deviceID);
                    }
                }

                if (xml.Contains("Sign in with password"))
                {
                    ifm.ClickByText(deviceID, "Sign in with password");
                    await DelayWithPause(1500, token);
                    xml = Android.GetUIDumpSafe(deviceID);
                }

                // Nếu đang ở màn hình form đăng nhập (có ô email hoặc ô password)
                if (xml.Contains("signin_email_field") || xml.Contains("signin_password_field") || xml.Contains("Enter your ExpressVPN"))
                {
                    UpdateProcess(deviceID + " 🔑 Đang nhập tài khoản ExpressVPN (paste thẳng siêu nhanh)...");
                    if (!string.IsNullOrWhiteSpace(vpnAccount) && vpnAccount.Contains("|"))
                    {
                        var parts = vpnAccount.Split('|');
                        string vpnEmail = parts[0].Trim();
                        string vpnPass = parts[1].Trim();

                        EnterExpressVpnCredentials(deviceID, vpnEmail, vpnPass);
                        await DelayWithPause(3500, token);

                        // Xử lý các màn hình xin quyền sau khi bấm Sign In
                        await HandleVpnPermissionsAsync(deviceID, token);
                    }
                    else
                    {
                        UpdateProcess(deviceID + " ⚠️ Ô tài khoản ExpressVPN trống hoặc sai định dạng (email|pass)!");
                    }
                }

                // Xử lý các màn hình cấp quyền nếu có
                await HandleVpnPermissionsAsync(deviceID, token);

                await DelayWithPause(2000, token);
            }

            // 3. Đổi vị trí sang Random US City qua tab ALL LOCATIONS
            UpdateProcess(deviceID + " 🌐 Mở danh sách vị trí VPN...");
            bool clickedLocPicker = ClickByResourceId(deviceID, "vpn_location_picker_button");
            if (!clickedLocPicker)
            {
                if (!ifm.ClickByText(deviceID, "Change"))
                {
                    KAutoHelper.ADBHelper.Tap(deviceID, 720, 1232);
                }
            }
            await DelayWithPause(2500, token);

            // Bấm qua tab ALL LOCATIONS
            UpdateProcess(deviceID + " 📋 Chuyển sang tab ALL LOCATIONS...");
            if (!ClickByTextIndexParent(deviceID, "ALL LOCATIONS", 0, 5000))
            {
                KAutoHelper.ADBHelper.Tap(deviceID, 1080, 388);
            }
            await DelayWithPause(2500, token);

            // Kiểm tra danh sách United States
            string allLocXml = Android.GetUIDumpSafe(deviceID);
            var usaCities = ExtractUsaCities(allLocXml);

            if (usaCities.Count == 0)
            {
                UpdateProcess(deviceID + " 🇺🇸 Mở rộng danh mục United States...");
                Point? expBtn = GetExpandButtonForCountry(allLocXml, "United States");
                if (expBtn.HasValue)
                {
                    KAutoHelper.ADBHelper.Tap(deviceID, expBtn.Value.X, expBtn.Value.Y);
                }
                else
                {
                    KAutoHelper.ADBHelper.Tap(deviceID, 1328, 734);
                }
                await DelayWithPause(2500, token);
                allLocXml = Android.GetUIDumpSafe(deviceID);
                usaCities = ExtractUsaCities(allLocXml);
            }

            if (usaCities.Count > 0)
            {
                // Vuốt ngẫu nhiên từ 0 đến 3 lần để chọn được nhiều IP US khác nhau bên dưới
                int swipeCount = rd.Next(0, 4);
                if (swipeCount > 0)
                {
                    UpdateProcess(deviceID + $" 📜 Vuốt danh sách IP US ngẫu nhiên {swipeCount} lần để chọn vị trí phong phú...");
                    for (int s = 0; s < swipeCount; s++)
                    {
                        if (token.IsCancellationRequested) return false;
                        AdbShell(deviceID, "input swipe 720 2200 720 700 250");
                        await DelayWithPause(800, token);
                    }
                    allLocXml = Android.GetUIDumpSafe(deviceID);
                    var newCities = ExtractUsaCities(allLocXml);
                    if (newCities.Count > 0)
                    {
                        usaCities = newCities;
                    }
                }

                var chosen = usaCities[rd.Next(usaCities.Count)];
                UpdateProcess(deviceID + $" 🎯 Chọn vị trí US ngẫu nhiên: {chosen.Name}");
                KAutoHelper.ADBHelper.Tap(deviceID, chosen.X, chosen.Y);
                await DelayWithPause(1500, token);

                string confirmXml = Android.GetUIDumpSafe(deviceID);
                if (confirmXml.Contains("Changing Location?") || confirmXml.Contains("Your VPN will briefly disconnect"))
                {
                    UpdateProcess(deviceID + " 🔄 Xác nhận đổi vị trí (Continue)...");
                    if (!ifm.ClickByText(deviceID, "Continue"))
                    {
                        KAutoHelper.ADBHelper.Tap(deviceID, 1055, 1633);
                    }
                    await DelayWithPause(1500, token);
                }
            }
            else
            {
                UpdateProcess(deviceID + " ⚠️ Không tìm thấy danh sách thành phố US, kết nối vị trí hiện tại");
            }

            // 4. Chờ VPN kết nối đến khi Protected (tối ưu siêu nhanh qua tun0 và polling 800ms)
            UpdateProcess(deviceID + " ⏳ Đang kết nối VPN...");
            bool isProtected = false;
            for (int w = 0; w < 20; w++)
            {
                if (token.IsCancellationRequested) return false;
                await DelayWithPause(800, token);

                // 1. Kiểm tra interface tun0 của Android (cực nhanh ~50ms)
                string tunCheck = AdbShell(deviceID, "ip addr show tun0");
                bool tunUp = !string.IsNullOrWhiteSpace(tunCheck) && (tunCheck.Contains("inet ") || tunCheck.Contains("POINTOPOINT"));

                // 2. Kiểm tra nhanh giao diện ExpressVPN
                string chkXml = Android.GetUIDumpSafe(deviceID);

                // Nếu có popup cấp quyền hệ thống
                if (chkXml.Contains("Set Up Your VPN") || chkXml.Contains("Connection request") || chkXml.Contains("com.android.vpndialogs"))
                {
                    await HandleVpnPermissionsAsync(deviceID, token);
                    continue;
                }

                // Nếu đã kết nối thành công (qua tun0 hoặc text Protected)
                if (tunUp || chkXml.Contains("Protected") || chkXml.Contains("protected") || chkXml.Contains("you’re protected") || chkXml.Contains("you're protected"))
                {
                    isProtected = true;
                    // Bấm Close nhanh tại popup nếu có (không chờ lâu)
                    if (chkXml.Contains("you’re protected") || chkXml.Contains("you're protected") || chkXml.Contains("Close sheet") || chkXml.Contains("Close"))
                    {
                        if (!ClickByDumpXml(deviceID, "Close", 1, 300))
                        {
                            KAutoHelper.ADBHelper.Tap(deviceID, 720, 2614);
                        }
                    }
                    UpdateProcess(deviceID + " 🟢 Đã xác nhận trạng thái Protected siêu nhanh!");
                    break;
                }

                // Nếu chưa bật kết nối, bấm nút nguồn
                if ((chkXml.Contains("Not connected") || chkXml.Contains("Not protected")) && !chkXml.Contains("Connecting"))
                {
                    UpdateProcess(deviceID + " 👉 Bấm nút nguồn kết nối VPN...");
                    if (!ClickByResourceId(deviceID, "vpn_connect_button"))
                    {
                        KAutoHelper.ADBHelper.Tap(deviceID, 720, 686);
                    }
                }
            }

            // 5. Kiểm tra IP thiết bị và hoàn tất
            if (isProtected)
            {
                string newIp = await GetDeviceIPAsync(deviceID);
                if (!string.IsNullOrWhiteSpace(newIp))
                {
                    UpdateProcess(deviceID + $" ✅ ExpressVPN đã kết nối thành công! IP: {newIp}");
                }
                else
                {
                    UpdateProcess(deviceID + " ✅ ExpressVPN đã ở trạng thái Protected!");
                }
                return true;
            }
            else
            {
                UpdateProcess(deviceID + " ❌ LỖI: ExpressVPN chưa được bật thành công (không đạt trạng thái Protected)!");
                return false;
            }
        }

        public static Point? GetHmaCountryArrowPoint(string xml, string countryName)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                var countryNode = doc.SelectSingleNode($"//node[@text='{countryName}']");
                if (countryNode != null)
                {
                    string bounds = countryNode.Attributes?["bounds"]?.Value;
                    if (GetCenter(bounds, out int cx, out int cy))
                    {
                        XmlNode parent = countryNode.ParentNode;
                        if (parent != null)
                        {
                            var detailNode = parent.SelectSingleNode(".//node[contains(@resource-id, ':id/details')]");
                            if (detailNode != null)
                            {
                                string dBounds = detailNode.Attributes?["bounds"]?.Value;
                                if (GetCenter(dBounds, out int dx, out int dy))
                                    return new Point(dx, dy);
                            }
                        }
                        return new Point(1272, cy);
                    }
                }
            }
            catch { }
            return null;
        }

        public static List<CityLocation> ExtractHmaCities(string xml)
        {
            var list = new List<CityLocation>();
            if (string.IsNullOrWhiteSpace(xml)) return list;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                var nodes = doc.SelectNodes("//node[contains(@resource-id, ':id/title')]");
                if (nodes != null)
                {
                    foreach (XmlNode n in nodes)
                    {
                        string text = n.Attributes?["text"]?.Value?.Trim();
                        string bounds = n.Attributes?["bounds"]?.Value;
                        if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(bounds))
                        {
                            if (text.Equals("OR", StringComparison.OrdinalIgnoreCase)) continue;
                            if (GetCenter(bounds, out int cx, out int cy))
                            {
                                if (cy > 400 && cy < 2800 && text.Length >= 3)
                                {
                                    list.Add(new CityLocation { Name = text, X = cx, Y = cy });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi trích xuất danh sách thành phố HMA: " + ex.Message);
            }
            return list;
        }

        private bool ClickHmaConnectionRequestOk(string deviceID, string xml = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(xml))
                {
                    xml = Android.GetUIDumpSafe(deviceID);
                }

                if (!string.IsNullOrWhiteSpace(xml))
                {
                    // 1. Tìm chính xác nút OK hoặc button1 từ XML
                    var mBtn = Regex.Match(xml, @"<node[^>]*?(?:resource-id=""(?:android:id/button1|com\.android\.vpndialogs:id/button1)""|text=""OK"")[^>]*?bounds=""\[(\d+),(\d+)\]\[(\d+),(\d+)\]""", RegexOptions.IgnoreCase);
                    if (mBtn.Success)
                    {
                        int bx = (int.Parse(mBtn.Groups[1].Value) + int.Parse(mBtn.Groups[3].Value)) / 2;
                        int by = (int.Parse(mBtn.Groups[2].Value) + int.Parse(mBtn.Groups[4].Value)) / 2;
                        KAutoHelper.ADBHelper.Tap(deviceID, bx, by);
                        AdbShell(deviceID, "input keyevent 66");
                        return true;
                    }
                }

                // 2. Dự phòng qua resourceId hoặc text
                if (ClickByResourceId(deviceID, "android:id/button1", xml))
                {
                    AdbShell(deviceID, "input keyevent 66");
                    return true;
                }
                if (ifm.ClickByText(deviceID, "OK") || ClickByDumpXml(deviceID, "OK", 1, 300))
                {
                    AdbShell(deviceID, "input keyevent 66");
                    return true;
                }

                // 3. Fallback toạ độ chuẩn (cho máy Pixel / màn hình 1440x3120) và gửi phím Enter
                KAutoHelper.ADBHelper.Tap(deviceID, 1193, 1892);
                AdbShell(deviceID, "input keyevent 66");
                return true;
            }
            catch
            {
                return false;
            }
        }

        async Task<bool> SetupHmaVpnAsync(string deviceID, CancellationToken token)
        {
            UpdateProcess(deviceID + " 🛡️ [HMA] Bắt đầu thiết lập HMA VPN...");

            // 1. Mở app HMA
            AdbShell(deviceID, "monkey -p com.hidemyass.hidemyassprovpn -c android.intent.category.LAUNCHER 1");
            await DelayWithPause(2500, token);

            // 2. Chờ màn hình chính hoặc xử lý popup / quyền VPN
            for (int attempt = 0; attempt < 8; attempt++)
            {
                if (token.IsCancellationRequested) return false;

                string xml = Android.GetUIDumpSafe(deviceID);
                if (string.IsNullOrWhiteSpace(xml))
                {
                    await DelayWithPause(1000, token);
                    continue;
                }

                // Nếu có dialog cấp quyền VPN hệ thống
                if (xml.Contains("Connection request") || xml.Contains("com.android.vpndialogs") || xml.Contains("wants to set up a VPN connection") || xml.Contains("Yêu cầu kết nối"))
                {
                    UpdateProcess(deviceID + " 🟢 [HMA] Phát hiện popup Connection request -> Bấm OK...");
                    ClickHmaConnectionRequestOk(deviceID, xml);
                    await DelayWithPause(2000, token);
                    continue;
                }

                // Nếu có màn hình quảng cáo Stay secure at all times
                if (xml.Contains("Stay secure at all times") || xml.Contains("OverlayActivity") || xml.Contains("ACTIVATE AUTO CONNECT"))
                {
                    UpdateProcess(deviceID + " 🟢 [HMA] Đóng màn hình Stay secure...");
                    if (!ClickByResourceId(deviceID, "close", xml))
                    {
                        KAutoHelper.ADBHelper.Tap(deviceID, 84, 192);
                    }
                    AdbShell(deviceID, "input keyevent 4");
                    await DelayWithPause(1000, token);
                    continue;
                }

                // Nếu có các nút đóng popup
                if (xml.Contains("Dismiss") || xml.Contains("Close") || xml.Contains("Skip"))
                {
                    ClickByDumpXml(deviceID, "Dismiss", 1, 300);
                    ClickByDumpXml(deviceID, "Close", 1, 300);
                    ClickByDumpXml(deviceID, "Skip", 1, 300);
                    await DelayWithPause(1000, token);
                    xml = Android.GetUIDumpSafe(deviceID);
                }

                // Nếu đã thấy nút Location hoặc connect_button hoặc đang ở trong list
                if (xml.Contains("location_button") || xml.Contains("location_selector") || xml.Contains("Location") || xml.Contains("connect_button") || xml.Contains("tab_all"))
                {
                    break;
                }

                await DelayWithPause(1000, token);
            }

            // 3. Bấm vào Location
            string curXml = Android.GetUIDumpSafe(deviceID);
            if (!curXml.Contains("tab_all") && !(curXml.Contains("Quick Access") && curXml.Contains("All")))
            {
                UpdateProcess(deviceID + " 🌐 [HMA] Mở danh sách vị trí VPN...");
                bool clickedLoc = ClickByResourceId(deviceID, "location_button");
                if (!clickedLoc) clickedLoc = ClickByResourceId(deviceID, "location_selector");
                if (!clickedLoc) clickedLoc = ClickByDumpXml(deviceID, "Location", 1, 500);
                if (!clickedLoc)
                {
                    KAutoHelper.ADBHelper.Tap(deviceID, 720, 2342);
                }
                await DelayWithPause(2000, token);
            }

            // 4. Chuyển sang tab All
            UpdateProcess(deviceID + " 📋 [HMA] Chuyển sang tab All...");
            bool clickedAll = ClickByResourceId(deviceID, "tab_all");
            if (!clickedAll) clickedAll = ClickByDumpXml(deviceID, "All", 1, 500);
            if (!clickedAll)
            {
                KAutoHelper.ADBHelper.Tap(deviceID, 720, 416);
            }
            await DelayWithPause(2000, token);

            // 5. Vuốt xuống cuối tìm United States và bấm mũi tên
            UpdateProcess(deviceID + " 🇺🇸 [HMA] Vuốt xuống cuối tìm United States...");
            bool foundUs = false;
            for (int swipe = 0; swipe < 15; swipe++)
            {
                if (token.IsCancellationRequested) return false;

                string listXml = Android.GetUIDumpSafe(deviceID);
                if (!string.IsNullOrWhiteSpace(listXml) && listXml.Contains("United States"))
                {
                    foundUs = true;
                    UpdateProcess(deviceID + " 👉 [HMA] Đã tìm thấy United States, bấm mũi tên mở danh sách IP...");

                    Point? arrowPt = GetHmaCountryArrowPoint(listXml, "United States");
                    if (arrowPt.HasValue)
                    {
                        KAutoHelper.ADBHelper.Tap(deviceID, arrowPt.Value.X, arrowPt.Value.Y);
                    }
                    else
                    {
                        var mUs = Regex.Match(listXml, @"<node[^>]*?text=""United States""[^>]*?bounds=""\[(\d+),(\d+)\]\[(\d+),(\d+)\]""");
                        if (mUs.Success)
                        {
                            int y = (int.Parse(mUs.Groups[2].Value) + int.Parse(mUs.Groups[4].Value)) / 2;
                            KAutoHelper.ADBHelper.Tap(deviceID, 1272, y);
                        }
                        else
                        {
                            KAutoHelper.ADBHelper.Tap(deviceID, 1272, 2396);
                        }
                    }
                    await DelayWithPause(2000, token);
                    break;
                }

                // Vuốt danh sách xuống (swipe up trên màn hình)
                AdbShell(deviceID, "input swipe 720 2400 720 400 200");
                await DelayWithPause(400, token);
            }

            if (!foundUs)
            {
                UpdateProcess(deviceID + " ⚠️ [HMA] Không tìm thấy United States sau khi vuốt!");
                return false;
            }

            // 6. Danh sách IP / Thành phố US: Random vuốt và chọn 1 IP bất kỳ
            int randomSwipes = rd.Next(0, 4);
            if (randomSwipes > 0)
            {
                UpdateProcess(deviceID + $" 📜 [HMA] Vuốt danh sách IP US ngẫu nhiên {randomSwipes} lần...");
                for (int s = 0; s < randomSwipes; s++)
                {
                    if (token.IsCancellationRequested) return false;
                    AdbShell(deviceID, "input swipe 720 2200 720 700 250");
                    await DelayWithPause(600, token);
                }
            }

            string usCitiesXml = Android.GetUIDumpSafe(deviceID);
            var cities = ExtractHmaCities(usCitiesXml);
            if (cities.Count > 0)
            {
                var chosen = cities[rd.Next(cities.Count)];
                UpdateProcess(deviceID + $" 🎯 [HMA] Chọn vị trí US ngẫu nhiên: {chosen.Name}...");
                KAutoHelper.ADBHelper.Tap(deviceID, chosen.X, chosen.Y);
            }
            else
            {
                UpdateProcess(deviceID + " ⚠️ [HMA] Không parse được danh sách thành phố, chọn ngẫu nhiên vị trí US...");
                KAutoHelper.ADBHelper.Tap(deviceID, 720, 1800);
            }

            // 6.1 Đón đầu và bấm OK popup Connection request ngay lập tức sau khi chọn City
            for (int cr = 0; cr < 6; cr++)
            {
                if (token.IsCancellationRequested) return false;
                await DelayWithPause(700, token);
                string pXml = Android.GetUIDumpSafe(deviceID);
                if (!string.IsNullOrWhiteSpace(pXml) && (pXml.Contains("Connection request") || pXml.Contains("com.android.vpndialogs") || pXml.Contains("wants to set up a VPN connection") || pXml.Contains("Yêu cầu kết nối")))
                {
                    UpdateProcess(deviceID + " 🟢 [HMA] Phát hiện popup Connection request -> Bấm OK kết nối...");
                    ClickHmaConnectionRequestOk(deviceID, pXml);
                    await DelayWithPause(1500, token);
                    break;
                }
            }

            // 7. Chờ kết nối VPN thành công
            UpdateProcess(deviceID + " ⏳ [HMA] Đang kết nối VPN...");
            bool isConnected = false;
            for (int w = 0; w < 30; w++)
            {
                if (token.IsCancellationRequested) return false;
                await DelayWithPause(800, token);

                // 1. Kiểm tra interface tun của Android (VPN thực sự có IP)
                string tunCheck = AdbShell(deviceID, "ip -o addr show");
                bool tunUp = !string.IsNullOrWhiteSpace(tunCheck) && Regex.IsMatch(tunCheck, @"tun\d+.*?inet\s+\d+");

                // 2. Kiểm tra UI HMA
                string chkXml = Android.GetUIDumpSafe(deviceID);
                if (!string.IsNullOrWhiteSpace(chkXml))
                {
                    if (chkXml.Contains("Connection request") || chkXml.Contains("com.android.vpndialogs") || chkXml.Contains("wants to set up a VPN connection") || chkXml.Contains("Yêu cầu kết nối"))
                    {
                        UpdateProcess(deviceID + " 🟢 [HMA] Phát hiện popup Connection request -> Bấm OK...");
                        ClickHmaConnectionRequestOk(deviceID, chkXml);
                        await DelayWithPause(1500, token);
                        continue;
                    }

                    if (chkXml.Contains("Stay secure at all times") || chkXml.Contains("OverlayActivity") || chkXml.Contains("ACTIVATE AUTO CONNECT"))
                    {
                        UpdateProcess(deviceID + " 🟢 [HMA] Đóng màn hình Stay secure...");
                        if (!ClickByResourceId(deviceID, "close", chkXml))
                        {
                            KAutoHelper.ADBHelper.Tap(deviceID, 84, 192);
                        }
                        AdbShell(deviceID, "input keyevent 4");
                        await DelayWithPause(1000, token);
                    }

                    if (chkXml.Contains("text_on") || (chkXml.Contains("new_ip_value") && !chkXml.Contains("Obtaining")))
                    {
                        isConnected = true;
                        UpdateProcess(deviceID + " 🟢 [HMA] Đã xác nhận VPN kết nối thành công!");
                        break;
                    }
                }

                if (tunUp)
                {
                    isConnected = true;
                    UpdateProcess(deviceID + " 🟢 [HMA] Đã xác nhận VPN kết nối thành công qua tun interface!");
                    break;
                }

                // Nếu chưa tự bật, bấm nút connect_button
                if (!string.IsNullOrWhiteSpace(chkXml) && chkXml.Contains("connect_button") && !chkXml.Contains("text_on") && !chkXml.Contains("Obtaining"))
                {
                    ClickByResourceId(deviceID, "connect_button");
                }
            }

            if (isConnected)
            {
                // Đảm bảo không bị kẹt ở màn hình OverlayActivity nếu có
                string finalXml = Android.GetUIDumpSafe(deviceID);
                if (!string.IsNullOrWhiteSpace(finalXml) && (finalXml.Contains("Stay secure at all times") || finalXml.Contains("OverlayActivity")))
                {
                    if (!ClickByResourceId(deviceID, "close", finalXml))
                    {
                        KAutoHelper.ADBHelper.Tap(deviceID, 84, 192);
                    }
                    AdbShell(deviceID, "input keyevent 4");
                    await DelayWithPause(1000, token);
                }

                string newIp = await GetDeviceIPAsync(deviceID);
                if (!string.IsNullOrWhiteSpace(newIp))
                {
                    UpdateProcess(deviceID + $" ✅ HMA VPN đã kết nối thành công! IP: {newIp}");
                }
                else
                {
                    UpdateProcess(deviceID + " ✅ HMA VPN đã ở trạng thái kết nối!");
                }
                return true;
            }
            else
            {
                UpdateProcess(deviceID + " ❌ LỖI: HMA VPN chưa được bật thành công!");
                return false;
            }
        }

        async Task RunDevice(string deviceID, int id, CancellationToken token)
        {
            currentDeviceID.Value = deviceID;
            var devPauseEvt = devicePauseEvents.GetOrAdd(deviceID, _ => new ManualResetEventSlim(true));
            devPauseEvt.Set();
            devicePausedStatus[deviceID] = false;

            UpdateProcess(deviceID + " ✅ ĐÃ VÀO RunDevice");
            MuteDevice(deviceID);

            try
            {
                using (var img = CloneImg())
                {
                    while (!token.IsCancellationRequested)
                    {
                        DateTime start;
                        lock (deviceLock)
                        {
                            if (!deviceStartTime.TryGetValue(deviceID, out start))
                                start = DateTime.Now;
                        }

                        if (token.IsCancellationRequested) return;
                        CheckPause(token);

                        // Lấy email từ danh sách
                        string acc = GetNextEmail();
                        if (string.IsNullOrWhiteSpace(acc) || !acc.Contains("|"))
                        {
                            if (chkAutoBuy.Checked && !string.IsNullOrWhiteSpace(txtGmailVipApiKey.Text))
                            {
                                UpdateProcess(deviceID + " ⚡ Hết email trong hàng đợi -> Kích hoạt Tự mua email từ GmailVIP...");
                                UpdateDeviceLog(deviceID, "⚡ Đang mua thêm email...");
                                bool bought = await TryAutoBuyEmailsAsync(deviceID);
                                if (bought)
                                {
                                    acc = GetNextEmail();
                                }
                            }
                        }

                        if (string.IsNullOrWhiteSpace(acc) || !acc.Contains("|"))
                        {
                            UpdateProcess(deviceID + " ⚠️ Hết email hoặc định dạng sai - dừng thiết bị!");
                            UpdateDeviceLog(deviceID, "⚠️ Hết email - Đã dừng");
                            return;
                        }

                        string googleEmail = acc.Split('|')[0].Trim();
                        string googlePass = acc.Split('|')[1].Trim();

                        UpdateProcess(deviceID + " 📧 Sử dụng Google Email: " + googleEmail);

                        // --- BƯỚC 0: ĐĂNG NHẬP & KẾT NỐI VPN ĐỔI IP US ---
                        bool vpnOk = false;
                        if (SelectedVpnProvider == "HMA VPN")
                        {
                            vpnOk = await SetupHmaVpnAsync(deviceID, token);
                            if (!vpnOk)
                            {
                                UpdateProcess(deviceID + " ⚠️ HMA VPN kết nối lần 1 chưa thành công, thử lại lần 2...");
                                GoHome(deviceID);
                                await DelayWithPause(2000, token);
                                vpnOk = await SetupHmaVpnAsync(deviceID, token);
                            }

                            if (!vpnOk)
                            {
                                UpdateProcess(deviceID + " ⛔ Dừng tiến trình do HMA VPN chưa kết nối thành công!");
                                return;
                            }
                        }
                        else
                        {
                            string vpnAccount = VpnAccountText;
                            vpnOk = await SetupExpressVpnAsync(deviceID, vpnAccount, token);
                            if (!vpnOk)
                            {
                                UpdateProcess(deviceID + " ⚠️ ExpressVPN kết nối lần 1 chưa thành công, thử lại lần 2...");
                                GoHome(deviceID);
                                await DelayWithPause(2000, token);
                                vpnOk = await SetupExpressVpnAsync(deviceID, vpnAccount, token);
                            }

                            if (!vpnOk)
                            {
                                UpdateProcess(deviceID + " ⛔ Dừng tiến trình do ExpressVPN chưa kết nối thành công!");
                                return;
                            }
                        }
                        await DelayWithPause(1500, token);
                        await ForceLockPortraitAsync(deviceID);
                        await DelayWithPause(1000, token);
                        GoHome(deviceID); // Về màn hình chính qua lệnh Home, tuyệt đối không bấm trên màn hình
                        await DelayWithPause(1500, token);
                        // --- BƯỚC 1: ĐĂNG NHẬP GOOGLE VÀO THIẾT BỊ ---
                        // Tuyệt đối không kiểm tra 'type=com.google' vì Android luôn chứa ServiceInfo com.google mặc định kể cả khi có 0 tài khoản!
                        // Chỉ bỏ qua bước này nếu chính xác email này đã được thêm vào máy:
                        string preCheckAcc = AdbShell(deviceID, "dumpsys account");
                        if (!string.IsNullOrWhiteSpace(preCheckAcc) && preCheckAcc.ToLower().Contains(googleEmail.ToLower()))
                        {
                            UpdateProcess(deviceID + $" ✅ Tài khoản Google ({googleEmail}) đã có sẵn trên máy!");
                            goto da_co_google;
                        }

                        bool googleLoginSuccess = false;
                        bool needRestartFlow = false;

                        for (int retryGoogle = 0; retryGoogle < 5; retryGoogle++)
                        {
                            if (token.IsCancellationRequested) return;
                            CheckPause(token);

                            string curAccCheck = AdbShell(deviceID, "dumpsys account");
                            if (!string.IsNullOrWhiteSpace(curAccCheck) && curAccCheck.ToLower().Contains(googleEmail.ToLower()))
                            {
                                UpdateProcess(deviceID + $" ✅ Tài khoản Google ({googleEmail}) đã có sẵn trên máy!");
                                googleLoginSuccess = true;
                                break;
                            }

                            if (retryGoogle > 0)
                            {
                                UpdateProcess(deviceID + $" 🔄 [TỰ PHỤC HỒI] Thử lại đăng nhập Google lần {retryGoogle + 1}/5 -> Về Home dọn dẹp...");
                                GoHome(deviceID);
                                await DelayWithPause(1000, token);
                                DismissSystemPopups(deviceID);
                            }

                            // 1. Kiểm tra nếu đang vướng màn hình lỗi 'Couldn't sign in' -> Lưu email, Change Device và chạy lại từ đầu
                            if (IsGoogleErrorScreen(deviceID, img.google_error))
                            {
                                await HandleGoogleErrorAndChangeDeviceAsync(deviceID, acc, token);
                                needRestartFlow = true;
                                break;
                            }

                            // Mở màn hình Add Account Google nếu chưa ở trong màn hình nhập tài khoản
                            string initFocus = AdbShell(deviceID, "dumpsys window | grep mCurrentFocus");
                            if (!initFocus.Contains("MinuteMaidActivity"))
                            {
                                if (!initFocus.Contains("ChooseAccountActivity"))
                                {
                                    UpdateProcess(deviceID + " Mở setting thêm tài khoản Google...");
                                    AdbShell(deviceID, "am start -a android.settings.ADD_ACCOUNT_SETTINGS");
                                    await DelayWithPause(1000, token);
                                }

                                // Chọn Google (tọa độ chính xác 720, 841 trên màn hình Add an account)
                                if (!ifm.ClickByText(deviceID, "Google"))
                                {
                                    if (!ClickByDumpXml(deviceID, "Google", 1, 300))
                                    {
                                        KAutoHelper.ADBHelper.Tap(deviceID, 720, 841);
                                    }
                                }
                                if (token.IsCancellationRequested) return;
                                CheckPause(token);

                                // Chờ màn hình đăng nhập Google xuất hiện (poll nhanh 300ms)
                                for (int w = 0; w < 25; w++)
                                {
                                    if (token.IsCancellationRequested) return;
                                    CheckPause(token);
                                    string curFocus = AdbShell(deviceID, "dumpsys window | grep mCurrentFocus");
                                    if (curFocus.Contains("MinuteMaidActivity")) break;
                                    if (curFocus.Contains("ErrorActivity") || IsGoogleErrorScreen(deviceID, img.google_error))
                                    {
                                        await HandleGoogleErrorAndChangeDeviceAsync(deviceID, acc, token);
                                        needRestartFlow = true;
                                        break;
                                    }
                                    await DelayWithPause(300, token);
                                }
                                if (needRestartFlow) break;
                                await DelayWithPause(500, token);
                            }

                            // Nếu bị lỗi 'Couldn't sign in' ngay khi nạp Google -> Lưu email, Change Device và chạy lại từ đầu
                            if (IsGoogleErrorScreen(deviceID, img.google_error))
                            {
                                await HandleGoogleErrorAndChangeDeviceAsync(deviceID, acc, token);
                                needRestartFlow = true;
                                break;
                            }

                            if (token.IsCancellationRequested) return;
                            CheckPause(token);

                            // 2. Chờ form Google tải xong (khoảng 2.5s)
                            UpdateProcess(deviceID + " ⏳ Chờ màn hình đăng nhập Google nạp...");
                            await DelayWithPause(2500, token);

                            if (IsGoogleErrorScreen(deviceID, img.google_error))
                            {
                                await HandleGoogleErrorAndChangeDeviceAsync(deviceID, acc, token);
                                needRestartFlow = true;
                                break;
                            }

                            // 3. Tìm kiếm kỹ màn hình liên kết số điện thoại ("Sign in with ease") nhiều lần đề phòng mạng chậm
                            UpdateProcess(deviceID + " 🔍 Đang kiểm tra màn hình liên kết SĐT (tìm kiếm nhiều lần đề phòng mạng chậm)...");
                            bool hasPhoneScreen = false;
                            for (int chk = 0; chk < 10; chk++)
                            {
                                if (token.IsCancellationRequested) return;
                                CheckPause(token);

                                if (IsGoogleErrorScreen(deviceID, img.google_error))
                                {
                                    await HandleGoogleErrorAndChangeDeviceAsync(deviceID, acc, token);
                                    needRestartFlow = true;
                                    break;
                                }

                                // 1. So khớp ảnh skipphone
                                using (Bitmap mainBmp = SafeScreenShoot(deviceID))
                                {
                                    if (mainBmp != null && img.skipphone != null)
                                    {
                                        Point? pt = NeoX.ImageScanOpenCV.FindOutPoint(mainBmp, img.skipphone, 0.75);
                                        if (pt.HasValue)
                                        {
                                            hasPhoneScreen = true;
                                            break;
                                        }
                                    }
                                }

                                // 2. Kiểm tra text XML dự phòng
                                string xmlCheck = Android.GetUIDumpSafe(deviceID);
                                if (!string.IsNullOrWhiteSpace(xmlCheck))
                                {
                                    if (xmlCheck.Contains("Sign in with ease") || xmlCheck.Contains("search for accounts connected to this phone number"))
                                    {
                                        hasPhoneScreen = true;
                                        break;
                                    }

                                    // Nếu đã nạp hẳn sang màn hình nhập Email (có text "Email or phone" hoặc "Forgot email")
                                    if (xmlCheck.Contains("Email or phone") || xmlCheck.Contains("Forgot email") || xmlCheck.Contains("Use your Google Account"))
                                    {
                                        if (chk >= 2)
                                        {
                                            hasPhoneScreen = false;
                                            break;
                                        }
                                    }
                                }

                                await DelayWithPause(800, token);
                            }

                            if (needRestartFlow) break;

                            if (hasPhoneScreen)
                            {
                                UpdateProcess(deviceID + " 👉 Phát hiện màn hình liên kết SĐT -> Bấm nút Skip tại (132, 2778)...");
                                if (!ClickByDumpXml(deviceID, "Skip", 1, 300))
                                {
                                    KAutoHelper.ADBHelper.Tap(deviceID, 132, 2778);
                                }
                                await DelayWithPause(2500, token);
                            }
                            else
                            {
                                UpdateProcess(deviceID + " ✅ Đã kiểm tra kỹ -> Xác nhận đang ở màn hình nhập Email!");
                            }

                            if (token.IsCancellationRequested) return;
                            CheckPause(token);

                            if (IsGoogleErrorScreen(deviceID, img.google_error))
                            {
                                await HandleGoogleErrorAndChangeDeviceAsync(deviceID, acc, token);
                                needRestartFlow = true;
                                break;
                            }

                            // 4. Nhập Email
                            UpdateProcess(deviceID + $" ⚡ Nhập Google Email ({googleEmail})...");
                            KAutoHelper.ADBHelper.Tap(deviceID, 720, 1242);
                            await DelayWithPause(400, token);

                            // Nhập Email qua FastInputText
                            FastInputText(deviceID, googleEmail);
                            await DelayWithPause(400, token);

                            // Bấm Enter và Next để sang bước Password
                            Android.GuiKey(deviceID, ADBKey.KEYCODE_ENTER);
                            await DelayWithPause(200, token);
                            KAutoHelper.ADBHelper.Tap(deviceID, 1215, 2778);
                            if (token.IsCancellationRequested) return;
                            CheckPause(token);

                            // 5. Chờ nạp màn hình Password nhanh gọn
                            UpdateProcess(deviceID + " 🔍 Đang chờ tải màn hình Password...");
                            await DelayWithPause(500, token);

                            for (int wPass = 0; wPass < 25; wPass++)
                            {
                                if (token.IsCancellationRequested) return;
                                CheckPause(token);

                                // Ưu tiên kiểm tra password trước vì 95% trường hợp là load thành công
                                if (IsGooglePasswordScreen(deviceID, img.pass_show, img.pass_welcome))
                                {
                                    UpdateProcess(deviceID + " 🟢 Đã thấy màn hình nhập Password!");
                                    break;
                                }

                                if (IsGoogleErrorScreen(deviceID, img.google_error))
                                {
                                    await HandleGoogleErrorAndChangeDeviceAsync(deviceID, acc, token);
                                    needRestartFlow = true;
                                    break;
                                }

                                await DelayWithPause(400, token);
                            }

                            if (needRestartFlow) break;

                            // 6. Nhập Password cẩn thận, chờ UI và bàn phím focus hoàn toàn
                            await DelayWithPause(600, token);
                            UpdateProcess(deviceID + " ⚡ Focus ô nhập Password (720, 1065)...");
                            KAutoHelper.ADBHelper.Tap(deviceID, 720, 1065);
                            await DelayWithPause(800, token); // Đợi bàn phím ADB Keyboard và ô input sẵn sàng 100%

                            // Nhập Password
                            UpdateProcess(deviceID + " 🔑 Nhập Google Password...");
                            FastInputText(deviceID, googlePass);
                            await DelayWithPause(700, token);

                            // Bấm Enter và Next
                            UpdateProcess(deviceID + " 👉 Bấm Next sau khi nhập mật khẩu...");
                            Android.GuiKey(deviceID, ADBKey.KEYCODE_ENTER);
                            await DelayWithPause(300, token);
                            KAutoHelper.ADBHelper.Tap(deviceID, 1220, 2580);
                            KAutoHelper.ADBHelper.Tap(deviceID, 1220, 2750);
                            if (token.IsCancellationRequested) return;
                            CheckPause(token);
                            await DelayWithPause(2500, token);

                            // Kiểm tra màn hình lỗi hoặc báo sai mật khẩu
                            // "sau khi nhập mật khẩu xong check xem có bị sai mật khẩu không nếu không thì bắt đầu vuốt để tìm nút i under chứ không phải là check nút i under không thấy thì là sai mật khẩu"
                            bool hasPasswordError = false;
                            for (int chkPw = 0; chkPw < 3; chkPw++)
                            {
                                if (token.IsCancellationRequested) return;
                                CheckPause(token);

                                if (IsGooglePasswordError(deviceID, img.google_pw_error))
                                {
                                    hasPasswordError = true;
                                    break;
                                }

                                if (IsGoogleErrorScreen(deviceID, img.google_error))
                                {
                                    await HandleGoogleErrorAndChangeDeviceAsync(deviceID, acc, token);
                                    needRestartFlow = true;
                                    break;
                                }

                                // Nếu màn hình vẫn hiển thị ô nhập password và chưa hề có thông báo lỗi, gõ lại Enter/Next ở nhịp đầu phòng click hụt
                                if (chkPw == 0 && IsGooglePasswordScreen(deviceID, img.pass_show, img.pass_welcome))
                                {
                                    Android.GuiKey(deviceID, ADBKey.KEYCODE_ENTER);
                                    KAutoHelper.ADBHelper.Tap(deviceID, 1220, 2580);
                                    KAutoHelper.ADBHelper.Tap(deviceID, 1220, 2750);
                                }

                                await DelayWithPause(1000, token);
                            }

                            if (needRestartFlow) break;

                            // Nếu phát hiện thông báo sai mật khẩu thực sự:
                            if (hasPasswordError)
                            {
                                UpdateProcess(deviceID + " ⚠️ Nhập mật khẩu bị báo sai thực sự -> Thoát và quay lại đăng nhập từ đầu...");
                                // Tuyệt đối KHÔNG gọi force-stop com.google.android.gms để tránh ngắt kết nối VPN!
                                Android.GuiKey(deviceID, ADBKey.KEYCODE_BACK);
                                await DelayWithPause(500, token);
                                GoHome(deviceID);
                                await DelayWithPause(1500, token);
                                continue; // Quay lại đầu vòng lặp retryGoogle!
                            }

                            // 7. NẾU KHÔNG BÁO SAI MẬT KHẨU -> BẮT ĐẦU QUY TRÌNH VUỐT VÀ BẤM ĐIỀU KHOẢN GOOGLE TỐC ĐỘ CAO (I UNDERSTAND / I AGREE / MORE / ACCEPT)
                            // "thay vì dum xml kiểm tra phần tử nó quá lâu giờ bạn chỉ cần click đúng vào vị trí góc đó là được không cần kiểm tra gì nhiều.sau khi nhập mật khẩu xong vuốt 3 đến 5 lần xuống cuối và bấm 4 lần vào cùng vị trí đó là xong vì 4 nút nó cũng gần như là cùng tọa độ với nhau"
                            UpdateProcess(deviceID + " 📜 Mật khẩu hợp lệ -> Bắt đầu quy trình điều khoản Google tốc độ cao...");

                            int screenW = 1440, screenH = 2960;
                            try
                            {
                                string sizeStr = AdbShell(deviceID, "wm size");
                                var match = System.Text.RegularExpressions.Regex.Match(sizeStr, @"(\d+)\s*x\s*(\d+)");
                                if (match.Success)
                                {
                                    screenW = int.Parse(match.Groups[1].Value);
                                    screenH = int.Parse(match.Groups[2].Value);
                                }
                            }
                            catch { }

                            int swipeStartX = screenW / 2;
                            int swipeStartY = (int)(screenH * 0.78);
                            int swipeEndY = (int)(screenH * 0.20);
                            int btnCornerX = (int)(screenW * 0.835); // Vị trí góc dưới cùng bên phải (~1202 ở 1440p)
                            int btnCornerY = (int)(screenH * 0.935); // Vị trí góc dưới cùng bên phải (~2767 ở 2960p)

                            // Bước 1: Vuốt 4 lần xuống cuối theo yêu cầu (3 đến 5 lần)
                            UpdateProcess(deviceID + $" 📜 Vuốt 4 lần xuống cuối màn hình...");
                            for (int s = 0; s < 4; s++)
                            {
                                if (token.IsCancellationRequested) return;
                                CheckPause(token);
                                AdbShell(deviceID, $"input swipe {swipeStartX} {swipeStartY} {swipeStartX} {swipeEndY} 250");
                                await DelayWithPause(350, token);
                            }
                            await DelayWithPause(600, token);

                            // Bước 2: Bấm 4 lần liên tiếp vào đúng vị trí góc điều khoản
                            string[] buttonNames = { "I understand / I agree", "I agree", "More", "Accept" };
                            for (int clickIdx = 0; clickIdx < 4; clickIdx++)
                            {
                                if (token.IsCancellationRequested) return;
                                CheckPause(token);

                                if (IsGoogleErrorScreen(deviceID, img.google_error))
                                {
                                    await HandleGoogleErrorAndChangeDeviceAsync(deviceID, acc, token);
                                    needRestartFlow = true;
                                    break;
                                }

                                // Kiểm tra sớm dumpsys account: nếu tài khoản đã thêm xong thì dừng bấm
                                string checkAcc = AdbShell(deviceID, "dumpsys account");
                                if (!string.IsNullOrWhiteSpace(checkAcc) && checkAcc.ToLower().Contains(googleEmail.ToLower()))
                                {
                                    UpdateProcess(deviceID + " ✅ Phát hiện tài khoản Google đã được thêm vào máy sớm!");
                                    break;
                                }

                                UpdateProcess(deviceID + $" 👉 Bấm lần {clickIdx + 1}/4 vào góc ({btnCornerX}, {btnCornerY}) [{buttonNames[clickIdx]}]...");
                                KAutoHelper.ADBHelper.Tap(deviceID, btnCornerX, btnCornerY);

                                int waitMs = (clickIdx == 0 || clickIdx == 1) ? 1500 : 1200;
                                await DelayWithPause(waitMs, token);
                            }

                            if (needRestartFlow) break;

                            // Bước 3: Xác nhận hoàn tất đăng nhập tài khoản Google vào máy
                            bool agreeDone = false;
                            for (int attempt = 0; attempt < 8; attempt++)
                            {
                                if (token.IsCancellationRequested) return;
                                CheckPause(token);

                                if (IsGoogleErrorScreen(deviceID, img.google_error))
                                {
                                    await HandleGoogleErrorAndChangeDeviceAsync(deviceID, acc, token);
                                    needRestartFlow = true;
                                    break;
                                }

                                string accs = AdbShell(deviceID, "dumpsys account");
                                if (!string.IsNullOrWhiteSpace(accs) && accs.ToLower().Contains(googleEmail.ToLower()))
                                {
                                    UpdateProcess(deviceID + " ✅ Đã hoàn tất đăng nhập tài khoản Google vào máy! Gọi lệnh về Home...");
                                    GoHome(deviceID);
                                    await DelayWithPause(1000, token);
                                    agreeDone = true;
                                    googleLoginSuccess = true;
                                    break;
                                }

                                // Nếu chưa thấy tài khoản (do lag mạng hoặc còn nút), nhấp phụ thêm vào góc đó
                                if (attempt >= 1)
                                {
                                    KAutoHelper.ADBHelper.Tap(deviceID, btnCornerX, btnCornerY);
                                }

                                await DelayWithPause(1000, token);
                            }

                            if (needRestartFlow) break;

                            if (agreeDone || googleLoginSuccess)
                            {
                                break;
                            }
                        }

                        if (needRestartFlow)
                        {
                            UpdateProcess(deviceID + " 🔄 Khởi động lại toàn bộ quy trình từ BƯỚC 0 với thiết bị mới...");
                            continue;
                        }

                        // CHỐT CHẶN GATE 1: Xác minh tài khoản Google đã thực sự có trong máy
                        string finalAccCheck = AdbShell(deviceID, "dumpsys account");
                        if (string.IsNullOrWhiteSpace(finalAccCheck) || !finalAccCheck.ToLower().Contains(googleEmail.ToLower()))
                        {
                            CaptureErrorSnapshot(deviceID, "GoogleLogin_Failed_Final");
                            UpdateProcess(deviceID + $" ❌ LỖI: Đăng nhập Google ({googleEmail}) không thành công sau 5 lần thử -> Lưu email và Change Device...");
                            UpdateDeviceLog(deviceID, "❌ Lỗi đăng nhập Google");
                            await HandleGoogleErrorAndChangeDeviceAsync(deviceID, acc, token);
                            continue;
                        }

                    da_co_google:
                        await DelayWithPause(1000, token);
                        GoHome(deviceID); // Về Home qua lệnh ADB keyevent 3, tuyệt đối không bấm trên màn hình tránh chạm Camera
                        await DelayWithPause(1500, token);

                        // --- BƯỚC 2: MỞ TIKTOK & ĐĂNG KÝ VỚI GOOGLE (CÓ TỰ PHỤC HỒI NẾU TREO) ---
                        bool tiktokLoginSuccess = false;

                        for (int retryTikTokLogin = 0; retryTikTokLogin < 3; retryTikTokLogin++)
                        {
                            if (token.IsCancellationRequested) return;
                            CheckPause(token);
                            DismissSystemPopups(deviceID);

                            if (retryTikTokLogin > 0)
                            {
                                UpdateProcess(deviceID + $" 🔄 [TỰ PHỤC HỒI] Kẹt ở bước TikTok đăng nhập Google -> Khôi phục lần {retryTikTokLogin}/2: Buộc dừng TikTok, mở lại...");
                                CaptureErrorSnapshot(deviceID, $"TikTok_GoogleLogin_Timeout_L{retryTikTokLogin}");
                                Android.DungApp(deviceID, "com.zhiliaoapp.musically");
                                AdbShell(deviceID, "am force-stop com.zhiliaoapp.musically");
                                GoHome(deviceID);
                                await DelayWithPause(1500, token);
                            }

                            if (retryTikTokLogin == 0)
                            {
                                UpdateProcess(deviceID + " Xóa dữ liệu TikTok...");
                                Android.XoaDuLieu(deviceID, "com.zhiliaoapp.musically");
                                await DelayWithPause(1500, token);
                            }

                            UpdateProcess(deviceID + " Mở App TikTok...");
                            Android.MoApp(deviceID, "com.zhiliaoapp.musically");
                            await DelayWithPause(5000, token);

                            // Bỏ qua popup khi mở lại nếu có
                            string openXml = Android.GetUIDumpSafe(deviceID);
                            if (openXml.Contains("Start watching"))
                            {
                                ifm.ClickByText(deviceID, "Start watching");
                                await DelayWithPause(1500, token);
                            }
                            if (openXml.Contains("Not now"))
                            {
                                ifm.ClickByText(deviceID, "Not now");
                                await DelayWithPause(1000, token);
                            }

                            UpdateProcess(deviceID + " Click Tiếp tục với Google & kiểm tra hộp thoại chọn tài khoản...");
                            for (int g = 0; g < 10; g++)
                            {
                                if (token.IsCancellationRequested) return;
                                CheckPause(token);
                                DismissSystemPopups(deviceID);

                                string ttXml = Android.GetUIDumpSafe(deviceID);
                                if (!string.IsNullOrWhiteSpace(ttXml) && (ttXml.Contains("When’s your birthday?") || ttXml.Contains("birthday") || ttXml.Contains("Birthday") || ttXml.Contains("nickname") || ttXml.Contains("Nickname") || ttXml.Contains("Profile") || ttXml.Contains("Hồ sơ")))
                                {
                                    tiktokLoginSuccess = true;
                                    break;
                                }

                                if (!string.IsNullOrWhiteSpace(ttXml))
                                {
                                    if (ttXml.Contains("Choose an account") ||
                                        ttXml.Contains("Choose account") ||
                                        ttXml.Contains("account_picker") ||
                                        ttXml.Contains("account_name") ||
                                        ttXml.Contains("continue_button") ||
                                        ttXml.Contains("How it works") ||
                                        ttXml.Contains("You're in control") ||
                                        (!string.IsNullOrWhiteSpace(googleEmail) && ttXml.Contains(googleEmail)) ||
                                        ttXml.Contains("tux_dual_ball_loading"))
                                    {
                                        UpdateProcess(deviceID + " 🟢 Đã xuất hiện hộp thoại chọn tài khoản Google!");
                                        break;
                                    }
                                }

                                string winFocus = AdbShell(deviceID, "dumpsys window | grep -E 'mCurrentFocus|mFocusedApp'");
                                if (!string.IsNullOrWhiteSpace(winFocus) && (winFocus.Contains("SignInHubActivity") || winFocus.Contains("ChooseAccountActivity") || winFocus.Contains("com.google.android.gms")))
                                {
                                    UpdateProcess(deviceID + " 🟢 Phát hiện màn hình xác thực Google đang mở!");
                                    break;
                                }

                                UpdateProcess(deviceID + $" 👉 Hộp thoại chưa hiện, click 'Tiếp tục với Google' (lần {g + 1}/10)...");
                                bool clickedG = false;

                                if (!string.IsNullOrWhiteSpace(ttXml) && (ttXml.Contains("Continue with Google") || ttXml.Contains("Sign in with Google")))
                                {
                                    var mG = Regex.Match(ttXml, @"<node[^>]*?(?:text|content-desc)=""(?:Continue with Google|Sign in with Google)""[^>]*?bounds=""\[(\d+),(\d+)\]\[(\d+),(\d+)\]""");
                                    if (mG.Success)
                                    {
                                        int cx = (int.Parse(mG.Groups[1].Value) + int.Parse(mG.Groups[3].Value)) / 2;
                                        int cy = (int.Parse(mG.Groups[2].Value) + int.Parse(mG.Groups[4].Value)) / 2;
                                        KAutoHelper.ADBHelper.Tap(deviceID, cx, cy);
                                        clickedG = true;
                                    }

                                    if (!clickedG)
                                    {
                                        clickedG = ifm.ClickByText(deviceID, "Continue with Google");
                                    }
                                }

                                if (!clickedG)
                                {
                                    clickedG = SafeClickVaoAnh(deviceID, token, img.google);
                                }

                                if (!clickedG && IsCurrentFocus(deviceID, "com.zhiliaoapp.musically"))
                                {
                                    KAutoHelper.ADBHelper.Tap(deviceID, 720, 2297);
                                    await DelayWithPause(300, token);
                                    KAutoHelper.ADBHelper.Tap(deviceID, 720, 1229);
                                }

                                await DelayWithPause(2500, token);
                            }

                            if (tiktokLoginSuccess) break;

                            await DelayWithPause(1500, token);

                            // Xử lý hộp thoại Google Sign In / Chọn tài khoản & Consent sheet
                            UpdateProcess(deviceID + " Xử lý hộp thoại Google Sign In / Chọn tài khoản...");
                            for (int gConsent = 0; gConsent < 15; gConsent++)
                            {
                                if (token.IsCancellationRequested) return;
                                CheckPause(token);
                                DismissSystemPopups(deviceID);

                                string chXml = Android.GetUIDumpSafe(deviceID);
                                if (string.IsNullOrWhiteSpace(chXml))
                                {
                                    await DelayWithPause(1000, token);
                                    continue;
                                }

                                if (chXml.Contains("When’s your birthday?") || chXml.Contains("birthday") || chXml.Contains("Birthday") || chXml.Contains("nickname") || chXml.Contains("Nickname") || chXml.Contains("Profile") || chXml.Contains("Hồ sơ"))
                                {
                                    tiktokLoginSuccess = true;
                                    break;
                                }

                                // 1. Màn hình Google Consent bottom sheet
                                if (chXml.Contains("continue_button") || chXml.Contains("How it works") || chXml.Contains("You're in control"))
                                {
                                    UpdateProcess(deviceID + " 👉 Bấm Continue trên Google Identity bottom sheet...");
                                    if (!ClickByResourceId(deviceID, "continue_button", chXml))
                                    {
                                        if (!ifm.ClickByText(deviceID, "Continue"))
                                        {
                                            KAutoHelper.ADBHelper.Tap(deviceID, 720, 2768);
                                        }
                                    }
                                    await DelayWithPause(3000, token);
                                    continue;
                                }

                                // 2. Màn hình "Choose an account"
                                if (chXml.Contains("Choose an account") || chXml.Contains("Choose account") || chXml.Contains("account_picker_container") || chXml.Contains("account_name") || (!string.IsNullOrWhiteSpace(googleEmail) && chXml.Contains(googleEmail)))
                                {
                                    UpdateProcess(deviceID + " 👉 Chọn tài khoản trong hộp thoại Choose an account...");
                                    bool accSelected = false;

                                    if (!string.IsNullOrWhiteSpace(googleEmail) && chXml.Contains(googleEmail))
                                    {
                                        var mAcc = Regex.Match(chXml, $@"<node[^>]*?text=""{Regex.Escape(googleEmail)}""[^>]*?bounds=""\[(\d+),(\d+)\]\[(\d+),(\d+)\]""");
                                        if (mAcc.Success)
                                        {
                                            int cx = (int.Parse(mAcc.Groups[1].Value) + int.Parse(mAcc.Groups[3].Value)) / 2;
                                            int cy = (int.Parse(mAcc.Groups[2].Value) + int.Parse(mAcc.Groups[4].Value)) / 2;
                                            KAutoHelper.ADBHelper.Tap(deviceID, cx, cy);
                                            accSelected = true;
                                        }
                                    }

                                    if (!accSelected)
                                    {
                                        accSelected = ClickByResourceId(deviceID, "account_name", chXml);
                                    }
                                    if (!accSelected)
                                    {
                                        accSelected = ClickByResourceId(deviceID, "container", chXml);
                                    }
                                    if (!accSelected && (IsCurrentFocus(deviceID, "ChooseAccountActivity") || IsCurrentFocus(deviceID, "SignInHubActivity")))
                                    {
                                        KAutoHelper.ADBHelper.Tap(deviceID, 720, 1529);
                                    }

                                    await DelayWithPause(3500, token);
                                    continue;
                                }

                                // 3. Loading
                                if (chXml.Contains("tux_dual_ball_loading"))
                                {
                                    UpdateProcess(deviceID + " ⏳ Đang tải xác thực tài khoản Google...");
                                    await DelayWithPause(2000, token);
                                    continue;
                                }

                                // 4. Continue with Google
                                if (chXml.Contains("Continue with Google") || chXml.Contains("Sign in with Google"))
                                {
                                    UpdateProcess(deviceID + " 👉 Vẫn ở màn hình đăng nhập, click lại Continue with Google...");
                                    var mG = Regex.Match(chXml, @"<node[^>]*?(?:text|content-desc)=""(?:Continue with Google|Sign in with Google)""[^>]*?bounds=""\[(\d+),(\d+)\]\[(\d+),(\d+)\]""");
                                    if (mG.Success)
                                    {
                                        int cx = (int.Parse(mG.Groups[1].Value) + int.Parse(mG.Groups[3].Value)) / 2;
                                        int cy = (int.Parse(mG.Groups[2].Value) + int.Parse(mG.Groups[4].Value)) / 2;
                                        KAutoHelper.ADBHelper.Tap(deviceID, cx, cy);
                                    }
                                    else
                                    {
                                        if (!ifm.ClickByText(deviceID, "Continue with Google"))
                                        {
                                            if (IsCurrentFocus(deviceID, "com.zhiliaoapp.musically"))
                                            {
                                                KAutoHelper.ADBHelper.Tap(deviceID, 720, 2297);
                                            }
                                        }
                                    }
                                    await DelayWithPause(3000, token);
                                    continue;
                                }

                                await DelayWithPause(1500, token);
                            }

                            if (tiktokLoginSuccess) break;
                        }

                        // CHỐT CHẶN GATE 2: Bắt buộc phải vào được Birthday / Nickname / Profile
                        if (!tiktokLoginSuccess)
                        {
                            string chkAfterLogin = Android.GetUIDumpSafe(deviceID);
                            if (string.IsNullOrWhiteSpace(chkAfterLogin) || (!chkAfterLogin.Contains("birthday") && !chkAfterLogin.Contains("Birthday") && !chkAfterLogin.Contains("nickname") && !chkAfterLogin.Contains("Nickname") && !chkAfterLogin.Contains("Profile") && !chkAfterLogin.Contains("Hồ sơ")))
                            {
                                CaptureErrorSnapshot(deviceID, "TikTok_Login_Failed_Final");
                                UpdateProcess(deviceID + " ❌ LỖI NGHIÊM TRỌNG: TikTok không thể đăng nhập bằng Google sau các lần tự phục hồi -> Dừng chu kỳ thiết bị!");
                                UpdateDeviceLog(deviceID, "❌ Lỗi đăng nhập TikTok");
                                await HandleGoogleErrorAndChangeDeviceAsync(deviceID, acc, token);
                                continue;
                            }
                        }

                        // --- BƯỚC 3: NHẬP NGÀY THÁNG NĂM SINH (TUỔI > 20 NHƯNG KHÔNG QUÁ GIÀ) ---
                        string curXml3 = Android.GetUIDumpSafe(deviceID);
                        if (curXml3.Contains("When’s your birthday?") || curXml3.Contains("birthday") || curXml3.Contains("Birthday"))
                        {
                            UpdateProcess(deviceID + " Nhập ngày tháng năm sinh (tuổi > 20, không quá già)...");
                            SafeClickVaoAnh(deviceID, token, img.bsn);
                            await DelayWithPause(1000, token);

                            // Random tháng: 1 - 3 lần
                            int l1 = rd.Next(1, 4);
                            for (int i = 0; i < l1; i++)
                            {
                                Android.Swipe(deviceID, 370, 2050, 370, 2300);
                                await Task.Delay(rd.Next(300, 600));
                            }
                            // Random ngày: 1 - 4 lần
                            int l2 = rd.Next(1, 5);
                            for (int i = 0; i < l2; i++)
                            {
                                Android.Swipe(deviceID, 720, 2050, 720, 2300);
                                await Task.Delay(rd.Next(300, 600));
                            }
                            // Random năm: 7 - 9 lần (tương đương sinh năm 1997 - 2003, khoảng 23 - 29 tuổi, trên 20 và không quá già)
                            int l3 = rd.Next(7, 10);
                            for (int i = 0; i < l3; i++)
                            {
                                Android.Swipe(deviceID, 1070, 2050, 1070, 2350);
                                await Task.Delay(rd.Next(300, 600));
                            }
                            await DelayWithPause(1000, token);

                            UpdateProcess(deviceID + " Click Next/Continue sau khi nhập ngày sinh (hỗ trợ cả nút màu ĐỎ & màu ĐEN)...");
                            for (int btnAttempt = 0; btnAttempt < 3; btnAttempt++)
                            {
                                ClickTikTokPrimaryButton(deviceID, "Birthday Next", 0.28, 0.45, 720, 1040, img.tieptuc);
                                await DelayWithPause(3000, token);

                                string bCheckXml = Android.GetUIDumpSafe(deviceID);
                                if (!bCheckXml.Contains("When’s your birthday?") && !bCheckXml.Contains("birthday") && !bCheckXml.Contains("Birthday"))
                                {
                                    UpdateProcess(deviceID + " 🟢 Đã xác nhận ngày sinh và chuyển sang màn hình tiếp theo!");
                                    break;
                                }
                                UpdateProcess(deviceID + $" ⚠️ Vẫn còn ở màn hình ngày sinh (thử lại lần {btnAttempt + 2}/3)...");
                            }
                        }

                        // --- BƯỚC 4: TẠO NICKNAME ---
                        for (int nkAttempt = 0; nkAttempt < 3; nkAttempt++)
                        {
                            string nkXml = Android.GetUIDumpSafe(deviceID);
                            if (nkXml.Contains("nickname") || nkXml.Contains("Nickname") || nkXml.Contains("Create nickname") || nkXml.Contains("Create name"))
                            {
                                string nickname = Tiktok_helper.GenerateRandomNickname();
                                UpdateProcess(deviceID + " ✏️ Tạo Nickname: " + nickname);
                                ClickFirstEditText(deviceID, nkXml);
                                await DelayWithPause(300, token);

                                // Xóa toàn bộ tên cũ (tên mặc định từ Google)
                                string delKeys = string.Join(" ", Enumerable.Repeat("67", 50));
                                AdbShell(deviceID, $"input keyevent 123 {delKeys}");
                                await DelayWithPause(300, token);

                                // Nhập tên tiếng Anh ngẫu nhiên mới
                                FastInputText(deviceID, nickname);
                                await DelayWithPause(600, token);

                                // Bấm Enter trên bàn phím ảo
                                Android.GuiKey(deviceID, ADBKey.KEYCODE_ENTER);
                                await DelayWithPause(300, token);

                                // Ẩn bàn phím ảo để lộ nút Confirm/Continue màu đỏ
                                AdbShell(deviceID, "input keyevent 111");
                                Android.GuiKey(deviceID, ADBKey.KEYCODE_BACK);
                                await DelayWithPause(500, token);

                                // Bấm nút xác nhận Nickname
                                bool clickedConfirm = false;
                                string[] confirmTexts = new string[] { "Confirm", "Continue", "Confirm nickname", "Xác nhận", "Tiếp tục", "Done" };
                                foreach (var cText in confirmTexts)
                                {
                                    if (ifm.ClickByText(deviceID, cText) || ClickByDumpXml(deviceID, cText, 1, 200))
                                    {
                                        UpdateProcess(deviceID + $" 👉 Đã bấm xác nhận Nickname ({cText})!");
                                        clickedConfirm = true;
                                        break;
                                    }
                                }

                                if (!clickedConfirm)
                                {
                                    // Quét nút màu ĐỎ hoặc màu ĐEN của màn hình Nickname
                                    clickedConfirm = ClickTikTokPrimaryButton(deviceID, "Nickname Confirm", 0.45, 0.95, 720, 2593, img.tieptuc);
                                }

                                if (!clickedConfirm)
                                {
                                    // Bấm trực tiếp tọa độ nút Continue/Confirm ở đáy màn hình (720, 2593) và phía trên (720, 1550)
                                    UpdateProcess(deviceID + " 👉 Tap tọa độ xác nhận Nickname (720, 2593)...");
                                    KAutoHelper.ADBHelper.Tap(deviceID, 720, 2593);
                                    await DelayWithPause(400, token);
                                    KAutoHelper.ADBHelper.Tap(deviceID, 720, 1550);
                                }

                                await DelayWithPause(2500, token);

                                // Kiểm tra 2 trường hợp:
                                // TH1: Đã xác nhận thành công và nhảy qua màn hình tiếp theo (chọn sở thích) -> Thành công!
                                // TH2: Vẫn đứng yên ở màn hình Nickname (không tạo được) -> Bấm Skip fallback để nhảy qua màn hình chọn sở thích
                                string postNkXml = Android.GetUIDumpSafe(deviceID);
                                if (!string.IsNullOrWhiteSpace(postNkXml) && (postNkXml.Contains("nickname") || postNkXml.Contains("Nickname") || postNkXml.Contains("Create nickname") || postNkXml.Contains("Create name")))
                                {
                                    UpdateProcess(deviceID + " ⚠️ Không tạo được Nickname (vẫn đứng yên) -> Bấm Skip fallback để qua màn hình chọn sở thích...");
                                    if (!ifm.ClickByText(deviceID, "Skip"))
                                    {
                                        if (!ClickByDumpXml(deviceID, "Skip", 1, 300))
                                        {
                                            if (!ifm.ClickByText(deviceID, "Bỏ qua"))
                                            {
                                                ClickByDumpXml(deviceID, "Bỏ qua", 1, 300);
                                            }
                                        }
                                    }
                                    await DelayWithPause(2000, token);
                                }
                                else
                                {
                                    UpdateProcess(deviceID + " 🟢 Đặt Nickname thành công -> Đã chuyển sang màn hình tiếp theo!");
                                }
                                break;
                            }
                            await DelayWithPause(1500, token);
                        }

                        // --- BƯỚC 5: ĐÓNG VÀ MỞ LẠI TIKTOK (BỎ QUA CHỌN SỞ THÍCH) ---
                        UpdateProcess(deviceID + " Đóng app TikTok...");
                        Android.DungApp(deviceID, "com.zhiliaoapp.musically");
                        ADBHelper.ExecuteCMD("adb -s " + deviceID + " shell am force-stop com.zhiliaoapp.musically");
                        await DelayWithPause(2000, token);

                        UpdateProcess(deviceID + " Mở lại app TikTok...");
                        Android.MoApp(deviceID, "com.zhiliaoapp.musically");
                        await DelayWithPause(5000, token);

                        // Bỏ qua popup khi mở lại nếu có
                        string reXml = Android.GetUIDumpSafe(deviceID);
                        if (reXml.Contains("Start watching"))
                        {
                            ifm.ClickByText(deviceID, "Start watching");
                            await DelayWithPause(1500, token);
                        }
                        if (reXml.Contains("Not now"))
                        {
                            ifm.ClickByText(deviceID, "Not now");
                            await DelayWithPause(1000, token);
                        }

                        // Vuốt xem video 5 lần, mỗi lần cách nhau 2s
                        UpdateProcess(deviceID + " 📱 Vuốt xem video 5 lần (delay 2s/lần)...");
                        for (int i = 0; i < 5; i++)
                        {
                            if (token.IsCancellationRequested) return;
                            CheckPause(token);
                            Android.Swipe(deviceID, 698, 2018, 698, 572);
                            await DelayWithPause(2000, token);
                        }

                        // Kiểm tra màn hình xem còn hiện hướng dẫn vuốt không, nếu còn thì vuốt tiếp
                        for (int checkSwipe = 0; checkSwipe < 5; checkSwipe++)
                        {
                            if (token.IsCancellationRequested) return;
                            CheckPause(token);

                            string vXml = Android.GetUIDumpSafe(deviceID);
                            if (string.IsNullOrWhiteSpace(vXml))
                            {
                                await DelayWithPause(1000, token);
                                continue;
                            }

                            if (vXml.Contains("Start watching"))
                            {
                                ifm.ClickByText(deviceID, "Start watching");
                                await DelayWithPause(1500, token);
                            }
                            if (vXml.Contains("Not now"))
                            {
                                ifm.ClickByText(deviceID, "Not now");
                                await DelayWithPause(1000, token);
                            }

                            bool hasSwipeGuide = vXml.Contains("Swipe up") || vXml.ToLower().Contains("swipe up") ||
                                                 vXml.Contains("Vuốt lên") || vXml.ToLower().Contains("vuốt lên") ||
                                                 vXml.Contains("Swipe for more") || vXml.Contains("swipe_guide");

                            // Nếu còn hiện hướng dẫn vuốt hoặc chưa thấy thanh điều hướng Profile / Hồ sơ
                            if (hasSwipeGuide || (!vXml.Contains("Profile") && !vXml.Contains("Hồ sơ") && !vXml.Contains("Inbox") && !vXml.Contains("Friends")))
                            {
                                UpdateProcess(deviceID + " 👆 Vẫn còn hiện hướng dẫn vuốt video -> Tiếp tục vuốt thêm...");
                                Android.Swipe(deviceID, 698, 2018, 698, 572);
                                await DelayWithPause(2000, token);
                            }
                            else
                            {
                                UpdateProcess(deviceID + " 🎬 Đã hết hướng dẫn vuốt video -> Bắt đầu vào Hồ sơ.");
                                break;
                            }
                        }

                        // --- BƯỚC 6: VÀO PROFILE & LẤY USERNAME (GATE 1: USERNAME PRECONDITION) ---
                        UpdateProcess(deviceID + " Vào Profile...");
                        if (!ClickByDumpXml(deviceID, "Profile", 2))
                        {
                            if (!ifm.ClickByText(deviceID, "Profile"))
                            {
                                KAutoHelper.ADBHelper.Tap(deviceID, 1296, 2818);
                            }
                        }

                        // Yêu cầu: Sau khi vào hồ sơ chờ khoảng 3s, click lại vào Profile 1 lần nữa để tắt popup
                        UpdateProcess(deviceID + " Chờ 3s rồi click lại Profile để tắt popup...");
                        await DelayWithPause(3000, token);
                        if (!ClickByDumpXml(deviceID, "Profile", 1))
                        {
                            KAutoHelper.ADBHelper.Tap(deviceID, 1296, 2818);
                        }
                        await DelayWithPause(1500, token);

                        string username = "";
                        for (int u = 0; u < 10; u++)
                        {
                            if (token.IsCancellationRequested) return;
                            string pXml = Android.GetUIDumpSafe(deviceID);

                            // Đóng các popup cản trở nếu có
                            if (pXml.Contains("Not now")) ClickByDumpXml(deviceID, "Not now", 1);
                            else if (pXml.Contains("Cancel")) ClickByDumpXml(deviceID, "Cancel", 1);
                            else if (pXml.Contains("Update")) KAutoHelper.ADBHelper.TapByPercent(deviceID, 50.7, 66.2);

                            username = Tiktok_helper.GetUsernameFromXml(pXml);
                            if (string.IsNullOrEmpty(username))
                            {
                                var matchUser = Regex.Match(pXml, @"text=""@([a-zA-Z0-9_\.]{2,32})""");
                                if (matchUser.Success)
                                {
                                    username = matchUser.Groups[1].Value.Trim();
                                }
                            }

                            if (!string.IsNullOrEmpty(username))
                                break;

                            if (u == 4)
                            {
                                UpdateProcess(deviceID + " Đang tải lại tab Profile để quét Username...");
                                KAutoHelper.ADBHelper.Tap(deviceID, 1296, 2818);
                            }

                            await DelayWithPause(1500, token);
                        }

                        if (string.IsNullOrEmpty(username))
                        {
                            CaptureErrorSnapshot(deviceID, "TikTok_GetUsername_Failed");
                            UpdateProcess(deviceID + " ❌ LỖI NGHIÊM TRỌNG: Không thể lấy được Username từ Profile sau 15s -> Lưu email và Change Device...");
                            UpdateDeviceLog(deviceID, "❌ Lỗi quét Username TikTok");
                            await HandleGoogleErrorAndChangeDeviceAsync(deviceID, acc, token);
                            continue;
                        }
                        UpdateProcess(deviceID + $" ✅ Username xác thực thành công: @{username}");
                        await DelayWithPause(1500, token);

                        // --- BƯỚC 7: MỞ CÀI ĐẶT (SETTINGS AND PRIVACY) ---
                        DismissSystemPopups(deviceID);
                        UpdateProcess(deviceID + " Mở menu 3 sọc...");
                        if (!ClickByDumpXml(deviceID, "Profile menu", 2))
                        {
                            if (IsCurrentFocus(deviceID, "com.zhiliaoapp.musically"))
                            {
                                KAutoHelper.ADBHelper.Tap(deviceID, 1342, 199);
                            }
                        }
                        await DelayWithPause(2500, token);

                        UpdateProcess(deviceID + " Vào Settings and privacy...");
                        bool clickedSettings = ClickByDumpXml(deviceID, "Settings and privacy", 3);
                        if (!clickedSettings)
                        {
                            clickedSettings = ClickByDumpXml(deviceID, "Settings", 2);
                        }
                        if (!clickedSettings && IsCurrentFocus(deviceID, "com.zhiliaoapp.musically"))
                        {
                            KAutoHelper.ADBHelper.Tap(deviceID, 828, 1512);
                        }
                        await DelayWithPause(3000, token);

                        // --- BƯỚC 8: VÀO SECURITY & PERMISSIONS -> 2-STEP VERIFICATION ---
                        DismissSystemPopups(deviceID);
                        UpdateProcess(deviceID + " Dump XML tìm & bấm Security & permissions...");
                        bool clickedSec = ClickByDumpXml(deviceID, "Security & permissions", 4, 1000);
                        if (!clickedSec)
                        {
                            clickedSec = ClickByDumpXml(deviceID, "Security", 2, 1000);
                        }
                        if (!clickedSec && IsCurrentFocus(deviceID, "com.zhiliaoapp.musically"))
                        {
                            Android.Swipe(deviceID, 720, 2000, 720, 1400);
                            await DelayWithPause(1500, token);
                            clickedSec = ClickByDumpXml(deviceID, "Security & permissions", 2, 1000);
                        }
                        if (!clickedSec && IsCurrentFocus(deviceID, "com.zhiliaoapp.musically"))
                        {
                            KAutoHelper.ADBHelper.Tap(deviceID, 720, 2185);
                        }
                        await DelayWithPause(3000, token);

                        UpdateProcess(deviceID + " Dump XML tìm & bấm 2-step verification...");
                        bool clicked2Step = ClickByDumpXml(deviceID, "2-step verification", 4, 1000);
                        if (!clicked2Step)
                        {
                            clicked2Step = ifm.ClickByText(deviceID, "2-step verification");
                        }
                        if (!clicked2Step && IsCurrentFocus(deviceID, "com.zhiliaoapp.musically"))
                        {
                            KAutoHelper.ADBHelper.Tap(deviceID, 720, 1116);
                        }
                        await DelayWithPause(3000, token);

                        // --- BƯỚC 9: CẤU HÌNH CÁC PHƯƠNG THỨC XÁC MINH 2FA ---
                        UpdateProcess(deviceID + " Cấu hình phương thức 2FA (Authenticator + Password, bỏ Phone & Email)...");
                        await ConfigureTwoFactorMethods(deviceID, token);
                        await DelayWithPause(1200, token);

                        // Bấm Turn on
                        UpdateProcess(deviceID + " Bấm Turn on...");
                        if (!ifm.ClickByText(deviceID, "Turn on"))
                        {
                            if (IsCurrentFocus(deviceID, "com.zhiliaoapp.musically"))
                            {
                                KAutoHelper.ADBHelper.Tap(deviceID, 720, 2799);
                            }
                        }
                        await DelayWithPause(3500, token);

                        // --- BƯỚC 10: NHẬP MẬT KHẨU TIKTOK (GATE 2: PASSWORD GATE) ---
                        UpdateProcess(deviceID + " Chờ màn hình đặt mật khẩu TikTok...");
                        for (int pw = 0; pw < 8; pw++)
                        {
                            if (token.IsCancellationRequested) return;
                            string pwXml = Android.GetUIDumpSafe(deviceID);
                            if (pwXml.Contains("password") || pwXml.Contains("Password") || pwXml.Contains("android.widget.EditText") || pwXml.Contains("Continue"))
                            {
                                break;
                            }
                            await DelayWithPause(1200, token);
                        }

                        string tiktokPass = "Cocasocola@1";
                        UpdateProcess(deviceID + " Nhập mật khẩu TikTok...");
                        ClickFirstEditText(deviceID, Android.GetUIDumpSafe(deviceID));
                        await DelayWithPause(500, token);
                        Android.NhapTextDelay(deviceID, tiktokPass, 150);
                        UpdateProcess(deviceID + " Mật khẩu đặt là: " + tiktokPass);
                        await DelayWithPause(1000, token);

                        UpdateProcess(deviceID + " Bấm Continue sau khi nhập mật khẩu (hỗ trợ cả nút màu ĐỎ & màu ĐEN)...");
                        ClickTikTokPrimaryButton(deviceID, "Password Continue", 0.40, 0.95, 720, 2593, img.tieptuc);

                        // Chốt chặn Gate 2: Bắt buộc phải chờ màn hình đặt mật khẩu hoàn tất và chuyển sang màn hình 2FA
                        bool passPassed = false;
                        for (int cp = 0; cp < 10; cp++)
                        {
                            if (token.IsCancellationRequested) return;
                            await DelayWithPause(1500, token);
                            string checkXml = Android.GetUIDumpSafe(deviceID);
                            if (checkXml.Contains("2-step verification setup") || checkXml.Contains("Authenticator") || checkXml.Contains("Copy key") || checkXml.Contains(":id/zaq"))
                            {
                                passPassed = true;
                                break;
                            }

                            string lowerXml = checkXml.ToLower();
                            if (lowerXml.Contains("couldn't") || lowerXml.Contains("can't") || lowerXml.Contains("unable") || lowerXml.Contains("error") || lowerXml.Contains("không thể") || lowerXml.Contains("thử lại"))
                            {
                                UpdateProcess(deviceID + " ⚠️ Phát hiện TikTok báo lỗi không tạo được mật khẩu!");
                                break;
                            }

                            if (checkXml.Contains("Continue"))
                            {
                                ifm.ClickByText(deviceID, "Continue");
                                KAutoHelper.ADBHelper.Tap(deviceID, 720, 2593);
                            }
                        }

                        if (!passPassed)
                        {
                            // Người dùng yêu cầu: nếu có lúc báo không tạo được mật khẩu thì hãy tắt app tiktok xong mở lại chạy lại từ đầu từ phần vào tiktok vào hồ sơ vào setting vào bật 2fa và tiếp các phần sau đến khi hoàn thành 1 vòng
                            for (int retryPw = 1; retryPw <= 3 && !passPassed; retryPw++)
                            {
                                if (token.IsCancellationRequested) return;
                                UpdateProcess(deviceID + $" ⚠️ Không tạo được mật khẩu TikTok -> Tắt app TikTok, mở lại, vào Hồ sơ -> Cài đặt -> Bật 2FA (Lần thử {retryPw}/3)...");

                                // 1. Tắt app TikTok
                                AdbShell(deviceID, "am force-stop com.zhiliaoapp.musically");
                                await DelayWithPause(2000, token);

                                // 2. Mở lại TikTok
                                Android.MoApp(deviceID, "com.zhiliaoapp.musically");
                                await DelayWithPause(5000, token);

                                // Tắt popup khi mở lại nếu có
                                string rePopXml = Android.GetUIDumpSafe(deviceID);
                                if (rePopXml.Contains("Start watching"))
                                {
                                    ifm.ClickByText(deviceID, "Start watching");
                                    await DelayWithPause(1500, token);
                                }
                                if (rePopXml.Contains("Not now"))
                                {
                                    ifm.ClickByText(deviceID, "Not now");
                                    await DelayWithPause(1000, token);
                                }

                                // 3. Vào lại Hồ sơ (Profile)
                                UpdateProcess(deviceID + " Vào lại tab Hồ sơ...");
                                for (int p = 0; p < 8; p++)
                                {
                                    if (token.IsCancellationRequested) return;
                                    string pXml = Android.GetUIDumpSafe(deviceID);
                                    if (pXml.Contains("Profile menu") || pXml.Contains("Edit profile") || pXml.Contains("Add bio") || pXml.Contains("@"))
                                    {
                                        break;
                                    }
                                    if (pXml.Contains("Don't allow")) ClickByDumpXml(deviceID, "Don't allow", 1);
                                    else if (pXml.Contains("Cancel")) ClickByDumpXml(deviceID, "Cancel", 1);
                                    else if (pXml.Contains("Skip")) ClickByDumpXml(deviceID, "Skip", 1);
                                    else if (pXml.Contains("Not now")) ClickByDumpXml(deviceID, "Not now", 1);

                                    KAutoHelper.ADBHelper.Tap(deviceID, 1296, 2818);
                                    await DelayWithPause(2000, token);
                                }

                                await DelayWithPause(2000, token);
                                KAutoHelper.ADBHelper.Tap(deviceID, 1296, 2818);
                                await DelayWithPause(1500, token);

                                // Quét lại username nếu chưa có
                                if (string.IsNullOrEmpty(username))
                                {
                                    string pXml2 = Android.GetUIDumpSafe(deviceID);
                                    username = Tiktok_helper.GetUsernameFromXml(pXml2);
                                    if (string.IsNullOrEmpty(username))
                                    {
                                        var matchUser = Regex.Match(pXml2, @"text=""@([a-zA-Z0-9_\.]{2,32})""");
                                        if (matchUser.Success) username = matchUser.Groups[1].Value.Trim();
                                    }
                                }

                                // 4. Mở menu 3 sọc -> Settings and privacy
                                UpdateProcess(deviceID + " Mở menu 3 sọc...");
                                if (!ClickByDumpXml(deviceID, "Profile menu", 2))
                                {
                                    KAutoHelper.ADBHelper.Tap(deviceID, 1342, 199);
                                }
                                await DelayWithPause(2500, token);

                                UpdateProcess(deviceID + " Vào Settings and privacy...");
                                bool reClickedSettings = ClickByDumpXml(deviceID, "Settings and privacy", 3);
                                if (!reClickedSettings) reClickedSettings = ClickByDumpXml(deviceID, "Settings", 2);
                                if (!reClickedSettings) KAutoHelper.ADBHelper.Tap(deviceID, 828, 1512);
                                await DelayWithPause(3000, token);

                                // 5. Vào Security & permissions
                                UpdateProcess(deviceID + " Vào Security & permissions...");
                                bool reClickedSec = ClickByDumpXml(deviceID, "Security & permissions", 4, 1000);
                                if (!reClickedSec) reClickedSec = ClickByDumpXml(deviceID, "Security", 2, 1000);
                                if (!reClickedSec)
                                {
                                    Android.Swipe(deviceID, 720, 2000, 720, 1400);
                                    await DelayWithPause(1500, token);
                                    reClickedSec = ClickByDumpXml(deviceID, "Security & permissions", 2, 1000);
                                }
                                if (!reClickedSec) KAutoHelper.ADBHelper.Tap(deviceID, 720, 2185);
                                await DelayWithPause(3000, token);

                                // 6. Vào 2-step verification
                                UpdateProcess(deviceID + " Vào 2-step verification...");
                                bool reClicked2Step = ClickByDumpXml(deviceID, "2-step verification", 4, 1000);
                                if (!reClicked2Step) reClicked2Step = ifm.ClickByText(deviceID, "2-step verification");
                                if (!reClicked2Step) KAutoHelper.ADBHelper.Tap(deviceID, 720, 1116);
                                await DelayWithPause(3000, token);

                                // 7. Cấu hình phương thức 2FA (Authenticator + Password, bỏ Phone & Email)
                                UpdateProcess(deviceID + " Cấu hình phương thức 2FA (Authenticator + Password, bỏ Phone & Email)...");
                                await ConfigureTwoFactorMethods(deviceID, token);
                                await DelayWithPause(1200, token);

                                // Bấm Turn on
                                UpdateProcess(deviceID + " Bấm Turn on...");
                                if (!ifm.ClickByText(deviceID, "Turn on"))
                                {
                                    KAutoHelper.ADBHelper.Tap(deviceID, 720, 2799);
                                }
                                await DelayWithPause(3500, token);

                                // 8. Đặt lại mật khẩu
                                UpdateProcess(deviceID + " Chờ màn hình đặt mật khẩu TikTok...");
                                bool needPassAgain = false;
                                for (int pw = 0; pw < 8; pw++)
                                {
                                    if (token.IsCancellationRequested) return;
                                    string pwXml = Android.GetUIDumpSafe(deviceID);
                                    if (pwXml.Contains("2-step verification setup") || pwXml.Contains("Authenticator") || pwXml.Contains("Copy key") || pwXml.Contains(":id/zaq"))
                                    {
                                        passPassed = true;
                                        break;
                                    }
                                    if (pwXml.Contains("password") || pwXml.Contains("Password") || pwXml.Contains("android.widget.EditText") || pwXml.Contains("Continue"))
                                    {
                                        needPassAgain = true;
                                        break;
                                    }
                                    await DelayWithPause(1200, token);
                                }

                                if (!passPassed && needPassAgain)
                                {
                                    UpdateProcess(deviceID + " Nhập lại mật khẩu TikTok...");
                                    ClickFirstEditText(deviceID, Android.GetUIDumpSafe(deviceID));
                                    await DelayWithPause(500, token);
                                    Android.NhapTextDelay(deviceID, tiktokPass, 150);
                                    await DelayWithPause(1000, token);

                                    UpdateProcess(deviceID + " Bấm Continue sau khi nhập mật khẩu...");
                                    if (!ifm.ClickByText(deviceID, "Continue"))
                                    {
                                        if (!ifm.ClickByText(deviceID, "Next"))
                                        {
                                            KAutoHelper.ADBHelper.Tap(deviceID, 720, 2593);
                                        }
                                    }

                                    for (int cp = 0; cp < 10; cp++)
                                    {
                                        if (token.IsCancellationRequested) return;
                                        await DelayWithPause(1500, token);
                                        string checkXml = Android.GetUIDumpSafe(deviceID);
                                        if (checkXml.Contains("2-step verification setup") || checkXml.Contains("Authenticator") || checkXml.Contains("Copy key") || checkXml.Contains(":id/zaq"))
                                        {
                                            passPassed = true;
                                            break;
                                        }

                                        string lowerXml = checkXml.ToLower();
                                        if (lowerXml.Contains("couldn't") || lowerXml.Contains("can't") || lowerXml.Contains("unable") || lowerXml.Contains("error") || lowerXml.Contains("không thể") || lowerXml.Contains("thử lại"))
                                        {
                                            UpdateProcess(deviceID + " ⚠️ Vẫn báo lỗi đặt mật khẩu!");
                                            break;
                                        }

                                        if (checkXml.Contains("Continue"))
                                        {
                                            ifm.ClickByText(deviceID, "Continue");
                                            KAutoHelper.ADBHelper.Tap(deviceID, 720, 2593);
                                        }
                                    }
                                }
                            }
                        }

                        if (!passPassed)
                        {
                            CaptureErrorSnapshot(deviceID, "TikTok_SetPassword_Failed_Final");
                            UpdateProcess(deviceID + " ❌ LỖI: Đã thử lại 3 lần nhưng không thể vượt qua bước đặt mật khẩu TikTok -> Lưu email và Change Device...");
                            UpdateDeviceLog(deviceID, "❌ Lỗi đặt mật khẩu TikTok");
                            await HandleGoogleErrorAndChangeDeviceAsync(deviceID, acc, token);
                            continue;
                        }
                        UpdateProcess(deviceID + " ✅ Đã đặt mật khẩu thành công và chuyển sang màn hình 2FA Setup!");

                        // --- BƯỚC 11: TRÍCH XUẤT 2FA KEY & NHẬP OTP (GATE 3: KEY & GATE 4: OTP) ---
                        UpdateProcess(deviceID + " 🔍 Đang chờ tải và trích xuất 2FA Secret Key...");
                        string key2fa = "";
                        for (int k = 0; k < 20; k++)
                        {
                            if (token.IsCancellationRequested) return;
                            string authXml = Android.GetUIDumpSafe(deviceID);
                            key2fa = Tiktok_helper.Get2FAKeyFromXml(authXml);
                            if (string.IsNullOrEmpty(key2fa))
                            {
                                key2fa = Get2FA(authXml);
                            }

                            if (!string.IsNullOrEmpty(key2fa) && key2fa.Length >= 16)
                            {
                                break;
                            }

                            await DelayWithPause(1500, token);
                        }

                        // CHỐT CHẶN GATE 3: Bắt buộc phải có 2FA Secret Key hợp lệ
                        if (string.IsNullOrEmpty(key2fa) || key2fa.Length < 16)
                        {
                            CaptureErrorSnapshot(deviceID, "TikTok_ExtractKey2FA_Failed");
                            UpdateProcess(deviceID + " ❌ LỖI NGHIÊM TRỌNG: Không thể trích xuất chuỗi 2FA Secret Key sau 30s -> Lưu email và Change Device...");
                            UpdateDeviceLog(deviceID, "❌ Lỗi trích xuất 2FA Key");
                            await HandleGoogleErrorAndChangeDeviceAsync(deviceID, acc, token);
                            continue;
                        }
                        UpdateProcess(deviceID + $" 🔑 Trích xuất 2FA Key thành công: {key2fa}");

                        // Bấm Next chuyển sang màn hình nhập mã OTP
                        UpdateProcess(deviceID + " Bấm Next chuyển sang màn hình nhập OTP...");
                        if (!ClickByDumpXml(deviceID, "Next", 2))
                        {
                            if (!ifm.ClickByText(deviceID, "Next"))
                            {
                                KAutoHelper.ADBHelper.Tap(deviceID, 720, 2743);
                            }
                        }

                        // Chờ màn hình nhập OTP xuất hiện
                        bool atOtpScreen = false;
                        for (int o = 0; o < 10; o++)
                        {
                            if (token.IsCancellationRequested) return;
                            await DelayWithPause(1500, token);
                            string otpXml = Android.GetUIDumpSafe(deviceID);
                            if (otpXml.Contains("Your code was sent") || otpXml.Contains("Authenticator app") || (otpXml.Contains("android.widget.EditText") && !otpXml.Contains("Copy key")))
                            {
                                atOtpScreen = true;
                                break;
                            }

                            if (otpXml.Contains("Copy key") || otpXml.Contains("Next"))
                            {
                                KAutoHelper.ADBHelper.Tap(deviceID, 720, 2743);
                            }
                        }

                        if (!atOtpScreen)
                        {
                            CaptureErrorSnapshot(deviceID, "TikTok_OtpScreen_Failed");
                            UpdateProcess(deviceID + " ❌ LỖI: Không thể chuyển sang màn hình nhập OTP 2FA sau 15s -> Lưu email và Change Device...");
                            UpdateDeviceLog(deviceID, "❌ Lỗi màn hình OTP 2FA");
                            await HandleGoogleErrorAndChangeDeviceAsync(deviceID, acc, token);
                            continue;
                        }
                        UpdateProcess(deviceID + " ✅ Đã sang màn hình nhập OTP Authenticator!");

                        // Focus vào ô OTP và nhập mã OTP TOTP mới nhất
                        UpdateProcess(deviceID + " Tap focus vào ô nhập OTP...");
                        ClickFirstEditText(deviceID, Android.GetUIDumpSafe(deviceID));
                        KAutoHelper.ADBHelper.Tap(deviceID, 720, 826);
                        await DelayWithPause(600, token);

                        string otpCode = Tiktok_helper.GenerateTOTP(key2fa);
                        if (string.IsNullOrEmpty(otpCode))
                        {
                            otpCode = GetOTP(key2fa);
                        }
                        if (string.IsNullOrEmpty(otpCode))
                        {
                            UpdateProcess(deviceID + " ❌ LỖI: Không thể tính toán mã OTP TOTP!");
                            return;
                        }
                        UpdateProcess(deviceID + $" 🔢 Tính toán mã OTP mới nhất ({otpCode}) -> Đang nhập vào ô xác minh...");
                        Android.NhapTextDelay(deviceID, otpCode, 150);
                        await DelayWithPause(2500, token);

                        // --- BƯỚC 12: BẤM SKIP CÁC BƯỚC PHỤ & XÁC NHẬN 2FA KÍCH HOẠT THÀNH CÔNG 100% ---
                        UpdateProcess(deviceID + " Kiểm tra xác nhận kích hoạt 2FA và Skip các bước phụ...");
                        bool is2FaActivated = false;

                        for (int sk = 0; sk < 12; sk++)
                        {
                            if (token.IsCancellationRequested) return;
                            string skXml = Android.GetUIDumpSafe(deviceID);

                            // 1. Chốt chặn thành công: Màn hình hiển thị "2-step verification is on" hoặc có "Turn off" hoặc vào TwoStepVerificationManagementActivity
                            if (skXml.Contains("2-step verification is on") || skXml.Contains("Turn off") || skXml.Contains("TwoStepVerificationManagementActivity"))
                            {
                                is2FaActivated = true;
                                UpdateProcess(deviceID + " ✅ XÁC NHẬN: 2-step verification đã kích hoạt THÀNH CÔNG 100%!");
                                break;
                            }

                            // 2. Màn hình phụ: Add phone, Add to trusted devices hoặc có nút Skip
                            if (skXml.Contains("Skip") || skXml.Contains("Add phone") || skXml.Contains("trusted devices"))
                            {
                                UpdateProcess(deviceID + " 👉 Bấm Skip qua bước phụ...");
                                if (!ifm.ClickByText(deviceID, "Skip"))
                                {
                                    if (!ClickByDumpXml(deviceID, "Skip", 1))
                                    {
                                        KAutoHelper.ADBHelper.Tap(deviceID, 1331, 199);
                                    }
                                }
                                await DelayWithPause(2000, token);
                                continue;
                            }

                            // 3. Nếu bị báo sai mã OTP ("Enter a valid code")
                            if (skXml.Contains("Enter a valid code"))
                            {
                                UpdateProcess(deviceID + " ❌ Bị báo sai OTP ('Enter a valid code') -> Chuyển sang quy trình khôi phục theo yêu cầu!");
                                break;
                            }

                            // 4. Nếu vẫn còn ở màn hình OTP (do mã OTP cũ hết hạn hoặc gõ thiếu số):
                            if (skXml.Contains("Your code was sent") || skXml.Contains("Authenticator app"))
                            {
                                if (sk % 3 == 0 && sk > 0)
                                {
                                    UpdateProcess(deviceID + " Vẫn còn ở màn hình OTP -> Thử tạo OTP mới và gõ lại...");
                                    KAutoHelper.ADBHelper.Tap(deviceID, 720, 826);
                                    await DelayWithPause(400, token);
                                    string freshOtp = Tiktok_helper.GenerateTOTP(key2fa);
                                    Android.NhapTextDelay(deviceID, freshOtp, 150);
                                    await DelayWithPause(2000, token);
                                }
                            }

                            await DelayWithPause(1500, token);
                        }

                        // Nếu chưa kích hoạt được 2FA hoặc bị báo sai OTP -> Thực hiện quy trình khôi phục theo yêu cầu
                        if (!is2FaActivated)
                        {
                            UpdateProcess(deviceID + " ⚠️ Bắt đầu quy trình khôi phục 2FA: Tắt app TikTok, mở lại, cấu hình Authenticator và nhập OTP...");
                            bool recovered = await HandleWrongOtpAndReconfigure2FaAsync(deviceID, username, tiktokPass, token, (newKey) => { key2fa = newKey; });
                            if (recovered)
                            {
                                is2FaActivated = true;
                                UpdateProcess(deviceID + " 🎉 Quy trình khôi phục 2FA THÀNH CÔNG RỰC RỠ!");
                            }
                        }

                        // CHỐT CHẶN BẮT BUỘC: Nếu chưa kích hoạt 2FA thành công -> DỪNG, KHÔNG LƯU ACC!
                        if (!is2FaActivated)
                        {
                            CaptureErrorSnapshot(deviceID, "TikTok_2FA_NotActivated_Final");
                            UpdateProcess(deviceID + " ❌ LỖI NGHIÊM TRỌNG: 2FA chưa kích hoạt thành công sau quy trình khôi phục -> Lưu email và Change Device...");
                            UpdateDeviceLog(deviceID, "❌ Lỗi kích hoạt 2FA");
                            await HandleGoogleErrorAndChangeDeviceAsync(deviceID, acc, token);
                            continue;
                        }

                        // --- BƯỚC 13: LƯU TÀI KHOẢN HOÀN CHỈNH (ĐỦ USERNAME, PASS, 2FA KEY & ĐÃ KÍCH HOẠT) ---
                        string accData = $"{username}|{tiktokPass}|{key2fa}";

                        SaveAccount(accData);
                        SaveCreatedAccount(accData);
                        UpdateProcess(deviceID + $" 🎉 ĐÃ TẠO VÀ LƯU XONG ACC CHUẨN 100%: {accData}");

                        // --- BƯỚC 14: UNLINK EMAIL (NẾU ĐƯỢC) ---
                        bool unlinked = await UnlinkEmailAsync(deviceID, token);
                        string unlinkStatus = unlinked ? "ok" : "no unlink";

                        // --- GHI DỮ LIỆU LÊN GOOGLE SHEET (VÀ BACKUP CỤC BỘ) ---
                        bool autoSendSheet = false;
                        if (chkAutoSendSheet.InvokeRequired)
                            autoSendSheet = (bool)chkAutoSendSheet.Invoke(new Func<bool>(() => chkAutoSendSheet.Checked));
                        else
                            autoSendSheet = chkAutoSendSheet.Checked;

                        if (autoSendSheet)
                        {
                            UpdateProcess(deviceID + " 📊 Đang ghi thông tin tài khoản lên Google Sheet...");
                            bool sheetSent = await SendToGoogleSheetAsync(googleEmail, googlePass, username, tiktokPass, key2fa, unlinkStatus);
                            if (sheetSent)
                            {
                                UpdateProcess(deviceID + " ✅ Đã ghi tài khoản lên Google Sheet thành công!");
                            }
                            else
                            {
                                UpdateProcess(deviceID + " ⚠️ Không thể ghi lên Google Sheet (đã lưu backup vào file accounts_full.txt)!");
                            }
                        }
                        else
                        {
                            SaveAccountFullBackup(googleEmail, googlePass, username, tiktokPass, key2fa, unlinkStatus);
                        }

                        // --- BƯỚC 15: SIGN OUT VPN (NẾU DÙNG EXPRESSVPN) ---
                        if (SelectedVpnProvider == "ExpressVPN")
                        {
                            await SignOutExpressVpnAsync(deviceID, token);
                        }
                        else
                        {
                            UpdateProcess(deviceID + " ℹ️ Đang dùng HMA VPN: Bỏ qua Sign Out theo cấu hình.");
                        }

                        // Kiểm tra nếu đã hết email trong danh sách thì thử Auto Buy hoặc dừng lại
                        if (!HasRemainingEmails())
                        {
                            if (chkAutoBuy.Checked && !string.IsNullOrWhiteSpace(txtGmailVipApiKey.Text))
                            {
                                UpdateProcess(deviceID + " ⚡ Hết email -> Kích hoạt Tự mua email từ GmailVIP trước khi Change Device...");
                                UpdateDeviceLog(deviceID, "⚡ Đang mua thêm email...");
                                await TryAutoBuyEmailsAsync(deviceID);
                            }
                        }

                        if (!HasRemainingEmails())
                        {
                            UpdateProcess(deviceID + " 🏁 Đã hết email trong danh sách -> Hoàn tất tiến trình, dừng thiết bị (không cần Change Device)!");
                            UpdateDeviceLog(deviceID, "🏁 Hết email - Hoàn tất tiến trình");
                            return;
                        }

                        // --- BƯỚC 16: CHANGE DEVICE QUA API (US, Android 15) ---
                        bool changed = await ChangeDeviceAsync(deviceID, token);
                        if (!changed)
                        {
                            UpdateProcess(deviceID + " ⚠️ Change device không thành công, tạm dừng 10s trước khi bắt đầu chu kỳ mới...");
                            await DelayWithPause(10000, token);
                        }

                        UpdateProcess(deviceID + " 🔄 Hoàn tất 1 chu kỳ! Chờ 5s và tiếp tục lặp lại quy trình từ đầu...");
                        await DelayWithPause(5000, token);

                        lock (deviceLock)
                        {
                            deviceStartTime[deviceID] = DateTime.Now;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                UpdateProcess(deviceID + " ⛔ Đã hủy");
            }
            catch (Exception ex)
            {
                UpdateProcess(deviceID + " ❌ Lỗi trong RunDevice: " + ex.Message);
            }
            finally
            {
                lock (deviceLock)
                {
                    deviceTokens.Remove(deviceID);
                    deviceStartTime.Remove(deviceID);
                    devicePauseEvents.TryRemove(deviceID, out _);
                    devicePausedStatus.TryRemove(deviceID, out _);

                    if (deviceTokens.Count == 0)
                    {
                        isRunning = false;
                        if (button1.InvokeRequired)
                        {
                            button1.Invoke(new Action(() => button1.Enabled = true));
                        }
                        else
                        {
                            button1.Enabled = true;
                        }
                    }
                }

                UpdateProcess(deviceID + " 🧹 Đã cleanup");
            }
        }








        static Random rd = new Random();

        async Task DelayWithPause(int ms, CancellationToken token)
        {
            const int step = 200;
            int elapsed = 0;

            while (elapsed < ms)
            {
                token.ThrowIfCancellationRequested();
                CheckPause(token);

                int wait = Math.Min(step, ms - elapsed);
                await Task.Delay(wait, token);
                elapsed += wait;
            }
        }

        private async Task ConfigureTwoFactorMethods(string deviceID, CancellationToken token)
        {
            try
            {
                // Chờ trang 2-Step Verification tải xong hoàn toàn (tối đa 8s)
                for (int w = 0; w < 4; w++)
                {
                    if (token.IsCancellationRequested) return;
                    string chkXml = Android.GetUIDumpSafe(deviceID);
                    if (!string.IsNullOrWhiteSpace(chkXml) && (chkXml.Contains("Authenticator") || chkXml.Contains("Turn on") || chkXml.Contains("Select at least 2 methods")))
                    {
                        break;
                    }
                    await DelayWithPause(1500, token);
                }

                // Thứ tự an toàn theo quy định TikTok "Select at least 2 methods":
                // 1. Bật Authenticator nếu chưa bật
                // 2. Bật Password nếu chưa bật
                // 3. Tắt Phone nếu đang bật
                // 4. Tắt Email nếu đang bật
                await SetMethodCheckedState(deviceID, "Authenticator", true, token);
                await DelayWithPause(800, token);
                await SetMethodCheckedState(deviceID, "Password", true, token);
                await DelayWithPause(800, token);
                await SetMethodCheckedState(deviceID, "Phone", false, token);
                await DelayWithPause(800, token);
                await SetMethodCheckedState(deviceID, "Email", false, token);
                await DelayWithPause(800, token);
            }
            catch (Exception ex)
            {
                UpdateProcess(deviceID + " Lỗi ConfigureTwoFactorMethods: " + ex.Message);
            }
        }

        private void ConfigureTwoFactorMethods(string deviceID)
        {
            ConfigureTwoFactorMethods(deviceID, CancellationToken.None).GetAwaiter().GetResult();
        }

        private async Task SetMethodCheckedState(string deviceID, string methodName, bool targetChecked, CancellationToken token)
        {
            try
            {
                string xml = Android.GetUIDumpSafe(deviceID);
                if (string.IsNullOrEmpty(xml))
                {
                    if (methodName.Equals("Authenticator", StringComparison.OrdinalIgnoreCase) && targetChecked) KAutoHelper.ADBHelper.Tap(deviceID, 1314, 1458);
                    else if (methodName.Equals("Password", StringComparison.OrdinalIgnoreCase) && targetChecked) KAutoHelper.ADBHelper.Tap(deviceID, 1314, 1738);
                    else if (methodName.Equals("Phone", StringComparison.OrdinalIgnoreCase) && !targetChecked) KAutoHelper.ADBHelper.Tap(deviceID, 1314, 898);
                    else if (methodName.Equals("Email", StringComparison.OrdinalIgnoreCase) && !targetChecked) KAutoHelper.ADBHelper.Tap(deviceID, 1314, 1178);
                    return;
                }

                var doc = XDocument.Parse(xml);
                var targetNode = doc.Descendants("node").FirstOrDefault(n =>
                {
                    string desc = n.Attribute("content-desc")?.Value ?? "";
                    string text = n.Attribute("text")?.Value ?? "";
                    return desc.StartsWith(methodName, StringComparison.OrdinalIgnoreCase) ||
                           text.Equals(methodName, StringComparison.OrdinalIgnoreCase);
                });

                if (targetNode == null)
                {
                    int fbX = 1314, fbY = 1458;
                    if (methodName.Equals("Authenticator", StringComparison.OrdinalIgnoreCase)) fbY = 1458;
                    else if (methodName.Equals("Password", StringComparison.OrdinalIgnoreCase)) fbY = 1738;
                    else if (methodName.Equals("Phone", StringComparison.OrdinalIgnoreCase)) fbY = 898;
                    else if (methodName.Equals("Email", StringComparison.OrdinalIgnoreCase)) fbY = 1178;

                    UpdateProcess(deviceID + $" 👉 Fallback {(targetChecked ? "Bật" : "Tắt")} {methodName} tại ({fbX}, {fbY})...");
                    KAutoHelper.ADBHelper.Tap(deviceID, fbX, fbY);
                    return;
                }

                var checkableNode = targetNode.Attribute("checkable")?.Value == "true"
                    ? targetNode
                    : targetNode.Ancestors("node").FirstOrDefault(a => a.Attribute("checkable")?.Value == "true")
                      ?? targetNode.Descendants("node").FirstOrDefault(d => d.Attribute("checkable")?.Value == "true");

                bool isChecked = false;
                if (checkableNode != null)
                {
                    isChecked = string.Equals(checkableNode.Attribute("checked")?.Value, "true", StringComparison.OrdinalIgnoreCase);
                }

                if (isChecked == targetChecked)
                {
                    UpdateProcess(deviceID + $" ✔️ {methodName} đã ở trạng thái {(targetChecked ? "CHECKED" : "UNCHECKED")}.");
                    return;
                }

                int tapX = 1314, tapY = 0;
                var cellRoot = checkableNode?.Ancestors("node").FirstOrDefault(a => a.Attribute("resource-id")?.Value?.Contains("two_sv_method_cell") == true)
                               ?? checkableNode ?? targetNode;

                var accessory = cellRoot.Descendants("node")
                    .FirstOrDefault(n => n.Attribute("resource-id")?.Value?.Contains("accessory") == true && !string.IsNullOrEmpty(n.Attribute("bounds")?.Value));

                if (accessory != null && GetCenter(accessory.Attribute("bounds")?.Value, out int ax, out int ay))
                {
                    tapX = ax;
                    tapY = ay;
                }
                else if (GetCenter(targetNode.Attribute("bounds")?.Value, out int tx, out int ty))
                {
                    tapY = ty;
                }
                else
                {
                    if (methodName.Equals("Authenticator", StringComparison.OrdinalIgnoreCase)) { tapX = 1314; tapY = 1458; }
                    else if (methodName.Equals("Password", StringComparison.OrdinalIgnoreCase)) { tapX = 1314; tapY = 1738; }
                    else if (methodName.Equals("Phone", StringComparison.OrdinalIgnoreCase)) { tapX = 1314; tapY = 898; }
                    else if (methodName.Equals("Email", StringComparison.OrdinalIgnoreCase)) { tapX = 1314; tapY = 1178; }
                }

                UpdateProcess(deviceID + $" 👉 {(targetChecked ? "Bật" : "Tắt")} {methodName} tại ({tapX}, {tapY})...");
                KAutoHelper.ADBHelper.Tap(deviceID, tapX, tapY);
            }
            catch (Exception ex)
            {
                UpdateProcess(deviceID + $" Lỗi SetMethodCheckedState({methodName}): {ex.Message}");
            }
        }

        async Task<bool> HandleWrongOtpAndReconfigure2FaAsync(string deviceID, string username, string tiktokPass, CancellationToken token, Action<string> onKeyExtracted = null)
        {
            try
            {
                UpdateProcess(deviceID + " ⚠️ Phát hiện lỗi sai mã OTP 2FA -> Khởi động quy trình khôi phục: Tắt app TikTok và mở lại...");

                // 1. Tắt app TikTok và mở lại
                AdbShell(deviceID, "am force-stop com.zhiliaoapp.musically");
                await DelayWithPause(2000, token);
                AdbShell(deviceID, "monkey -p com.zhiliaoapp.musically -c android.intent.category.LAUNCHER 1");
                await DelayWithPause(5000, token);

                // 2. Vào tab Profile (hồ sơ)
                UpdateProcess(deviceID + " Vào lại tab Profile...");
                for (int p = 0; p < 8; p++)
                {
                    if (token.IsCancellationRequested) return false;
                    string pXml = Android.GetUIDumpSafe(deviceID);
                    if (pXml.Contains("Profile menu") || pXml.Contains("Edit profile") || pXml.Contains("Add bio") || pXml.Contains("@"))
                    {
                        break;
                    }

                    // Tắt các popup nếu có
                    if (pXml.Contains("Don't allow")) ClickByDumpXml(deviceID, "Don't allow", 1);
                    else if (pXml.Contains("Cancel")) ClickByDumpXml(deviceID, "Cancel", 1);
                    else if (pXml.Contains("Skip")) ClickByDumpXml(deviceID, "Skip", 1);

                    KAutoHelper.ADBHelper.Tap(deviceID, 1296, 2818);
                    await DelayWithPause(2000, token);
                }

                // 3. Mở menu 3 gạch
                UpdateProcess(deviceID + " Mở menu 3 sọc...");
                if (!ClickByDumpXml(deviceID, "Profile menu", 2))
                {
                    KAutoHelper.ADBHelper.Tap(deviceID, 1342, 199);
                }
                await DelayWithPause(2500, token);

                // 4. Vào Settings and privacy
                UpdateProcess(deviceID + " Vào Settings and privacy...");
                if (!ClickByDumpXml(deviceID, "Settings and privacy", 3))
                {
                    if (!ClickByDumpXml(deviceID, "Settings", 2))
                    {
                        KAutoHelper.ADBHelper.Tap(deviceID, 828, 1512);
                    }
                }
                await DelayWithPause(3000, token);

                // 5. Vào Security & permissions
                UpdateProcess(deviceID + " Vào Security & permissions...");
                bool clickedSec = ClickByDumpXml(deviceID, "Security & permissions", 4, 1000);
                if (!clickedSec)
                {
                    clickedSec = ClickByDumpXml(deviceID, "Security", 2, 1000);
                }
                if (!clickedSec)
                {
                    Android.Swipe(deviceID, 720, 2000, 720, 1400);
                    await DelayWithPause(1500, token);
                    clickedSec = ClickByDumpXml(deviceID, "Security & permissions", 2, 1000);
                }
                if (!clickedSec)
                {
                    KAutoHelper.ADBHelper.Tap(deviceID, 720, 2185);
                }
                await DelayWithPause(3000, token);

                // 6. Vào 2-step verification
                UpdateProcess(deviceID + " Vào 2-step verification...");
                if (!ClickByDumpXml(deviceID, "2-step verification", 4, 1000))
                {
                    if (!ifm.ClickByText(deviceID, "2-step verification"))
                    {
                        KAutoHelper.ADBHelper.Tap(deviceID, 720, 1116);
                    }
                }
                await DelayWithPause(3500, token);

                // 7. Ở màn hình 2-step verification is off:
                // Theo yêu cầu: Tích chọn Authenticator, bỏ chọn Email (và Phone nếu có), giữ Password
                UpdateProcess(deviceID + " Cấu hình lại phương thức 2FA: Bật Authenticator, Bỏ tích Email...");
                for (int w = 0; w < 6; w++)
                {
                    if (token.IsCancellationRequested) return false;
                    string chkXml = Android.GetUIDumpSafe(deviceID);
                    if (chkXml.Contains("Select at least 2 methods") || chkXml.Contains("Authenticator") || chkXml.Contains("Turn on"))
                    {
                        break;
                    }
                    await DelayWithPause(1500, token);
                }

                // Thứ tự theo quy tắc TikTok (cần >= 2 phương thức):
                // Bật Authenticator trước (để luôn có >= 2 methods)
                await SetMethodCheckedState(deviceID, "Authenticator", true, token);
                await DelayWithPause(800, token);
                // Bật Password nếu chưa bật
                await SetMethodCheckedState(deviceID, "Password", true, token);
                await DelayWithPause(800, token);
                // Tắt Email
                await SetMethodCheckedState(deviceID, "Email", false, token);
                await DelayWithPause(800, token);
                // Tắt Phone nếu có
                await SetMethodCheckedState(deviceID, "Phone", false, token);
                await DelayWithPause(1000, token);

                // Bấm Turn on
                UpdateProcess(deviceID + " Bấm Turn on...");
                if (!ifm.ClickByText(deviceID, "Turn on"))
                {
                    KAutoHelper.ADBHelper.Tap(deviceID, 720, 2799);
                }
                await DelayWithPause(4000, token);

                // Kiểm tra xem có yêu cầu mật khẩu không (thường là đã có mật khẩu nên sẽ đi thẳng vào 2FA key)
                string afterTurnOnXml = Android.GetUIDumpSafe(deviceID);
                if (afterTurnOnXml.Contains("password") || afterTurnOnXml.Contains("Password") || afterTurnOnXml.Contains("Enter password") || afterTurnOnXml.Contains("Set password"))
                {
                    UpdateProcess(deviceID + " Nhập lại mật khẩu nếu TikTok yêu cầu...");
                    ClickFirstEditText(deviceID, afterTurnOnXml);
                    await DelayWithPause(500, token);
                    Android.NhapTextDelay(deviceID, tiktokPass, 150);
                    await DelayWithPause(800, token);
                    if (!ifm.ClickByText(deviceID, "Continue"))
                    {
                        if (!ifm.ClickByText(deviceID, "Next"))
                        {
                            KAutoHelper.ADBHelper.Tap(deviceID, 720, 2593);
                        }
                    }
                    await DelayWithPause(3500, token);
                }

                // 8. Trích xuất 2FA Secret Key mới
                UpdateProcess(deviceID + " 🔍 Đang trích xuất chuỗi 2FA Secret Key mới...");
                string newKey2fa = "";
                for (int k = 0; k < 20; k++)
                {
                    if (token.IsCancellationRequested) return false;
                    string authXml = Android.GetUIDumpSafe(deviceID);
                    newKey2fa = Tiktok_helper.Get2FAKeyFromXml(authXml);
                    if (string.IsNullOrEmpty(newKey2fa))
                    {
                        newKey2fa = Get2FA(authXml);
                    }

                    if (!string.IsNullOrEmpty(newKey2fa) && newKey2fa.Length >= 16)
                    {
                        break;
                    }

                    await DelayWithPause(1500, token);
                }

                if (string.IsNullOrEmpty(newKey2fa) || newKey2fa.Length < 16)
                {
                    UpdateProcess(deviceID + " ❌ Không thể trích xuất 2FA Secret Key mới!");
                    return false;
                }

                UpdateProcess(deviceID + $" 🔑 Trích xuất 2FA Key mới thành công: {newKey2fa}");
                if (onKeyExtracted != null)
                {
                    onKeyExtracted(newKey2fa);
                }

                // 9. Bấm Next sang màn hình nhập OTP
                UpdateProcess(deviceID + " Bấm Next sang màn hình nhập OTP...");
                if (!ClickByDumpXml(deviceID, "Next", 2))
                {
                    if (!ifm.ClickByText(deviceID, "Next"))
                    {
                        KAutoHelper.ADBHelper.Tap(deviceID, 720, 2743);
                    }
                }
                await DelayWithPause(3000, token);

                // Chờ màn hình nhập OTP
                for (int o = 0; o < 8; o++)
                {
                    if (token.IsCancellationRequested) return false;
                    string otpXml = Android.GetUIDumpSafe(deviceID);
                    if (otpXml.Contains("Your code was sent") || otpXml.Contains("Authenticator app") || (otpXml.Contains("android.widget.EditText") && !otpXml.Contains("Copy key")))
                    {
                        break;
                    }
                    if (otpXml.Contains("Copy key") || otpXml.Contains("Next"))
                    {
                        KAutoHelper.ADBHelper.Tap(deviceID, 720, 2743);
                    }
                    await DelayWithPause(1500, token);
                }

                // 10. Focus ô OTP và tính toán mã TOTP mới nhất để nhập
                UpdateProcess(deviceID + " Tap focus vào ô OTP và nhập mã OTP mới...");
                ClickFirstEditText(deviceID, Android.GetUIDumpSafe(deviceID));
                KAutoHelper.ADBHelper.Tap(deviceID, 720, 826);
                await DelayWithPause(600, token);

                string otpCode = Tiktok_helper.GenerateTOTP(newKey2fa);
                if (string.IsNullOrEmpty(otpCode)) otpCode = GetOTP(newKey2fa);
                UpdateProcess(deviceID + $" 🔢 Tính toán mã OTP mới ({otpCode}) -> Đang nhập vào ô xác minh...");
                Android.NhapTextDelay(deviceID, otpCode, 150);
                await DelayWithPause(3000, token);

                // 11. Bấm Skip qua các bước phụ (Add phone, Add to trusted devices) và kiểm tra kích hoạt thành công
                UpdateProcess(deviceID + " Kiểm tra và Skip qua các bước phụ...");
                bool isActivated = false;
                for (int sk = 0; sk < 12; sk++)
                {
                    if (token.IsCancellationRequested) return false;
                    string skXml = Android.GetUIDumpSafe(deviceID);

                    if (skXml.Contains("2-step verification is on") || skXml.Contains("Turn off") || skXml.Contains("TwoStepVerificationManagementActivity"))
                    {
                        isActivated = true;
                        UpdateProcess(deviceID + " ✅ XÁC NHẬN: 2-step verification đã kích hoạt THÀNH CÔNG 100%!");
                        break;
                    }

                    if (skXml.Contains("Skip") || skXml.Contains("Add phone") || skXml.Contains("trusted devices"))
                    {
                        UpdateProcess(deviceID + " 👉 Bấm Skip qua bước phụ...");
                        if (!ifm.ClickByText(deviceID, "Skip"))
                        {
                            if (!ClickByDumpXml(deviceID, "Skip", 1))
                            {
                                KAutoHelper.ADBHelper.Tap(deviceID, 1331, 199);
                            }
                        }
                        await DelayWithPause(2000, token);
                        continue;
                    }

                    await DelayWithPause(1500, token);
                }

                return isActivated;
            }
            catch (Exception ex)
            {
                UpdateProcess(deviceID + " ❌ Lỗi RecoverAndSetup2Fa: " + ex.Message);
                return false;
            }
        }

        async Task<bool> UnlinkEmailAsync(string deviceID, CancellationToken token)
        {
            try
            {
                UpdateProcess(deviceID + " 🔄 Bắt đầu kiểm tra Unlink email...");

                // 1. Nhấn Back 2 lần từ màn hình 2FA (hoặc Security) về Settings and privacy
                for (int b = 0; b < 2; b++)
                {
                    if (token.IsCancellationRequested) return false;
                    AdbShell(deviceID, "input keyevent 4");
                    await DelayWithPause(1500, token);
                }

                // Kiểm tra xem đã về Settings and privacy chưa, nếu chưa về bấm Back thêm
                for (int s = 0; s < 3; s++)
                {
                    string setXml = Android.GetUIDumpSafe(deviceID);
                    if (setXml.Contains("Settings and privacy") || setXml.Contains("Manage posts") || setXml.Contains("Security & permissions"))
                    {
                        break;
                    }
                    AdbShell(deviceID, "input keyevent 4");
                    await DelayWithPause(1500, token);
                }

                // 2. Bấm vào mục Account trong Settings and privacy
                UpdateProcess(deviceID + " 👉 Bấm vào Account...");
                bool clickedAcc = ClickAccountInSettings(deviceID);
                if (!clickedAcc)
                {
                    clickedAcc = ClickByDumpXml(deviceID, "Account", 2, 1000);
                }
                await DelayWithPause(2500, token);

                // 3. Bấm vào mục Account information
                UpdateProcess(deviceID + " 👉 Bấm vào Account information...");
                bool clickedAccInfo = ClickByDumpXml(deviceID, "Account information", 3, 1000);
                if (!clickedAccInfo)
                {
                    KAutoHelper.ADBHelper.Tap(deviceID, 720, 402);
                }
                await DelayWithPause(2500, token);

                // 4. Bấm vào mục Email
                UpdateProcess(deviceID + " 👉 Bấm vào Email...");
                bool clickedEmail = false;
                string accInfoXml = Android.GetUIDumpSafe(deviceID);
                if (!string.IsNullOrWhiteSpace(accInfoXml))
                {
                    var matches = Regex.Matches(accInfoXml, @"<node[^>]+?>");
                    foreach (Match m in matches)
                    {
                        string tag = m.Value;
                        var mBounds = Regex.Match(tag, @"bounds=""\[(\d+),(\d+)\]\[(\d+),(\d+)\]""");
                        if (!mBounds.Success) continue;

                        var mText = Regex.Match(tag, @"text=""([^""]*)""");
                        var mDesc = Regex.Match(tag, @"content-desc=""([^""]*)""");

                        string tVal = mText.Success ? mText.Groups[1].Value.Trim().ToLower() : "";
                        string dVal = mDesc.Success ? mDesc.Groups[1].Value.Trim().ToLower() : "";

                        if (tVal == "email" || dVal.StartsWith("email"))
                        {
                            int x1 = int.Parse(mBounds.Groups[1].Value);
                            int y1 = int.Parse(mBounds.Groups[2].Value);
                            int x2 = int.Parse(mBounds.Groups[3].Value);
                            int y2 = int.Parse(mBounds.Groups[4].Value);
                            if (x2 > x1 && y2 > y1)
                            {
                                KAutoHelper.ADBHelper.Tap(deviceID, (x1 + x2) / 2, (y1 + y2) / 2);
                                clickedEmail = true;
                                break;
                            }
                        }
                    }
                }

                if (!clickedEmail)
                {
                    KAutoHelper.ADBHelper.Tap(deviceID, 720, 584);
                }
                await DelayWithPause(2500, token);

                // 5. Kiểm tra popup: Có nút Unlink email hay không?
                string popupXml = Android.GetUIDumpSafe(deviceID);
                if (string.IsNullOrWhiteSpace(popupXml) || (!popupXml.Contains("Unlink email") && !popupXml.Contains("Unlink")))
                {
                    UpdateProcess(deviceID + " ⚠️ Không có tùy chọn Unlink email -> Bỏ qua bước Unlink.");
                    AdbShell(deviceID, "input keyevent 4");
                    AdbShell(deviceID, "am force-stop com.zhiliaoapp.musically");
                    await DelayWithPause(1000, token);
                    return false;
                }

                // Có nút Unlink email -> bấm vào Unlink email
                UpdateProcess(deviceID + " 👉 Bấm Unlink email...");
                bool clickedUnlink = ClickByDumpXml(deviceID, "Unlink email", 2, 1000);
                if (!clickedUnlink)
                {
                    clickedUnlink = ClickByDumpXml(deviceID, "Unlink", 2, 1000);
                }
                if (!clickedUnlink)
                {
                    KAutoHelper.ADBHelper.Tap(deviceID, 720, 2037);
                }
                await DelayWithPause(3000, token);

                // 6. Kiểm tra phản hồi sau khi bấm Unlink email:
                string confirmXml = Android.GetUIDumpSafe(deviceID);
                if (confirmXml.Contains("Remove email?") || confirmXml.Contains("unbind_confirm_continue") || confirmXml.Contains("Remove") || confirmXml.Contains("Continue to unlink"))
                {
                    UpdateProcess(deviceID + " 👉 Xác nhận gỡ email (Bấm Remove)...");
                    bool clickedRemove = false;
                    if (confirmXml.Contains("unbind_confirm_continue"))
                    {
                        var mId = Regex.Match(confirmXml, @"<node[^>]*?resource-id=""[^""]*unbind_confirm_continue""[^>]*?bounds=""\[(\d+),(\d+)\]\[(\d+),(\d+)\]""");
                        if (mId.Success)
                        {
                            int cx = (int.Parse(mId.Groups[1].Value) + int.Parse(mId.Groups[3].Value)) / 2;
                            int cy = (int.Parse(mId.Groups[2].Value) + int.Parse(mId.Groups[4].Value)) / 2;
                            KAutoHelper.ADBHelper.Tap(deviceID, cx, cy);
                            clickedRemove = true;
                        }
                    }

                    if (!clickedRemove)
                    {
                        clickedRemove = ClickByDumpXml(deviceID, "Remove", 2, 1000);
                    }
                    if (!clickedRemove)
                    {
                        clickedRemove = ClickByDumpXml(deviceID, "Continue to unlink", 2, 1000);
                    }
                    if (!clickedRemove)
                    {
                        KAutoHelper.ADBHelper.Tap(deviceID, 720, 2712);
                    }

                    await DelayWithPause(3000, token);
                    UpdateProcess(deviceID + " 🎉 Unlink email thành công! (Email unlinked)");
                    AdbShell(deviceID, "am force-stop com.zhiliaoapp.musically");
                    await DelayWithPause(1000, token);
                    return true;
                }
                else
                {
                    UpdateProcess(deviceID + " ⚠️ Không thể unlink email (TikTok yêu cầu giữ email hoặc thêm số điện thoại trước) -> Bỏ qua bước Unlink.");
                    AdbShell(deviceID, "am force-stop com.zhiliaoapp.musically");
                    await DelayWithPause(1000, token);
                    return false;
                }
            }
            catch (Exception ex)
            {
                UpdateProcess(deviceID + " ⚠️ Lỗi trong UnlinkEmail: " + ex.Message);
            }

            AdbShell(deviceID, "am force-stop com.zhiliaoapp.musically");
            return false;
        }

        async Task SignOutExpressVpnAsync(string deviceID, CancellationToken token)
        {
            try
            {
                UpdateProcess(deviceID + " 🔄 Mở ExpressVPN để Sign Out...");
                AdbShell(deviceID, "monkey -p com.expressvpn.vpn -c android.intent.category.LAUNCHER 1");
                await DelayWithPause(3500, token);

                // 1. Kiểm tra nếu đã ở màn hình đăng nhập (chưa đăng nhập hoặc đã Sign Out rồi)
                string initXml = Android.GetUIDumpSafe(deviceID);
                if (!string.IsNullOrWhiteSpace(initXml))
                {
                    if (initXml.Contains("Get ExpressVPN") || (initXml.Contains("Sign In") && !initXml.Contains("Profile")) || initXml.Contains("Have an account? Sign in"))
                    {
                        UpdateProcess(deviceID + " ℹ️ ExpressVPN đã ở trạng thái đăng xuất (Welcome/Sign In screen).");
                        return;
                    }

                    // Nếu có popup xuất hiện đè lên (vd Close, Skip, Dismiss)
                    if (initXml.Contains("Close") || initXml.Contains("Skip") || initXml.Contains("Dismiss"))
                    {
                        ClickByDumpXml(deviceID, "Close", 1, 500);
                        ClickByDumpXml(deviceID, "Skip", 1, 500);
                        await DelayWithPause(1500, token);
                    }
                }

                // 2. Bấm vào tab Profile
                UpdateProcess(deviceID + " 👉 Bấm vào tab Profile...");
                bool clickedProfile = false;
                string currentXml = Android.GetUIDumpSafe(deviceID);
                if (!string.IsNullOrWhiteSpace(currentXml) && currentXml.Contains("home_profile_tab"))
                {
                    var mProf = Regex.Match(currentXml, @"<node[^>]*?resource-id=""[^""]*home_profile_tab""[^>]*?bounds=""\[(\d+),(\d+)\]\[(\d+),(\d+)\]""");
                    if (mProf.Success)
                    {
                        int cx = (int.Parse(mProf.Groups[1].Value) + int.Parse(mProf.Groups[3].Value)) / 2;
                        int cy = (int.Parse(mProf.Groups[2].Value) + int.Parse(mProf.Groups[4].Value)) / 2;
                        KAutoHelper.ADBHelper.Tap(deviceID, cx, cy);
                        clickedProfile = true;
                    }
                }

                if (!clickedProfile)
                {
                    clickedProfile = ClickByDumpXml(deviceID, "Profile", 2, 1000);
                }

                if (!clickedProfile)
                {
                    KAutoHelper.ADBHelper.Tap(deviceID, 1307, 2822);
                    clickedProfile = true;
                }

                await DelayWithPause(2500, token);

                // 3. Vuốt xuống cuối trang Profile
                UpdateProcess(deviceID + " 📜 Vuốt xuống cuối trang Profile...");
                for (int sw = 0; sw < 2; sw++)
                {
                    if (token.IsCancellationRequested) return;
                    AdbShell(deviceID, "input swipe 720 2000 720 500 400");
                    await DelayWithPause(1000, token);
                }

                // 4. Bấm vào nút Sign Out
                UpdateProcess(deviceID + " 👉 Bấm Sign Out...");
                bool clickedSignOut = ClickByDumpXml(deviceID, "Sign Out", 3, 1000);
                if (!clickedSignOut)
                {
                    // Thử vuốt thêm 1 lần nữa nếu chưa chạm đáy
                    AdbShell(deviceID, "input swipe 720 2000 720 500 400");
                    await DelayWithPause(1000, token);
                    clickedSignOut = ClickByDumpXml(deviceID, "Sign Out", 2, 1000);
                }

                if (!clickedSignOut)
                {
                    KAutoHelper.ADBHelper.Tap(deviceID, 304, 2445);
                }

                await DelayWithPause(2000, token);

                // 5. Xác nhận Sign Out trên dialog popup
                for (int attempt = 0; attempt < 5; attempt++)
                {
                    if (token.IsCancellationRequested) return;

                    string dialogXml = Android.GetUIDumpSafe(deviceID);
                    if (string.IsNullOrWhiteSpace(dialogXml))
                    {
                        await DelayWithPause(1000, token);
                        continue;
                    }

                    // Nếu đã ra màn hình Welcome (đã Sign Out thành công)
                    if (dialogXml.Contains("Get ExpressVPN") || dialogXml.Contains("Have an account? Sign in"))
                    {
                        UpdateProcess(deviceID + " 🎉 Đã Sign Out ExpressVPN thành công!");
                        return;
                    }

                    // Nếu đang có popup hỏi xác nhận Sign Out
                    if (dialogXml.Contains("Are you sure") || dialogXml.Contains("CANCEL") || dialogXml.Contains("SIGN OUT"))
                    {
                        UpdateProcess(deviceID + " 👉 Xác nhận Sign Out (Bấm nút SIGN OUT trên popup)...");

                        bool clickedConfirm = false;

                        // Ưu tiên 1: Tìm node có text CHÍNH XÁC là "SIGN OUT" (phân biệt hoa thường để tránh bấm nhầm title "Sign Out")
                        var matches = Regex.Matches(dialogXml, @"<node[^>]+?>");
                        foreach (Match m in matches)
                        {
                            string tag = m.Value;
                            var mText = Regex.Match(tag, @"text=""([^""]*)""");
                            if (mText.Success && mText.Groups[1].Value == "SIGN OUT")
                            {
                                var mBounds = Regex.Match(tag, @"bounds=""\[(\d+),(\d+)\]\[(\d+),(\d+)\]""");
                                if (mBounds.Success)
                                {
                                    int cx = (int.Parse(mBounds.Groups[1].Value) + int.Parse(mBounds.Groups[3].Value)) / 2;
                                    int cy = (int.Parse(mBounds.Groups[2].Value) + int.Parse(mBounds.Groups[4].Value)) / 2;
                                    KAutoHelper.ADBHelper.Tap(deviceID, cx, cy);
                                    clickedConfirm = true;
                                    break;
                                }
                            }
                        }

                        // Ưu tiên 2: Tìm node nằm bên phải nút CANCEL
                        if (!clickedConfirm)
                        {
                            var mCancel = Regex.Match(dialogXml, @"<node[^>]*?text=""CANCEL""[^>]*?bounds=""\[(\d+),(\d+)\]\[(\d+),(\d+)\]""");
                            if (mCancel.Success)
                            {
                                int cY1 = int.Parse(mCancel.Groups[2].Value);
                                int cX2 = int.Parse(mCancel.Groups[3].Value);

                                foreach (Match m in matches)
                                {
                                    string tag = m.Value;
                                    if (tag.Contains("CANCEL")) continue;
                                    var mBounds = Regex.Match(tag, @"bounds=""\[(\d+),(\d+)\]\[(\d+),(\d+)\]""");
                                    if (mBounds.Success)
                                    {
                                        int x1 = int.Parse(mBounds.Groups[1].Value);
                                        int y1 = int.Parse(mBounds.Groups[2].Value);
                                        int x2 = int.Parse(mBounds.Groups[3].Value);
                                        int y2 = int.Parse(mBounds.Groups[4].Value);

                                        if (x1 >= cX2 && Math.Abs(y1 - cY1) < 100)
                                        {
                                            KAutoHelper.ADBHelper.Tap(deviceID, (x1 + x2) / 2, (y1 + y2) / 2);
                                            clickedConfirm = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        // Ưu tiên 3: Fallback tọa độ nút SIGN OUT trên popup
                        if (!clickedConfirm)
                        {
                            KAutoHelper.ADBHelper.Tap(deviceID, 973, 1659);
                        }

                        await DelayWithPause(3000, token);
                    }
                    else
                    {
                        // Chưa thấy popup -> có thể bấm Sign Out trượt -> bấm lại Sign Out
                        UpdateProcess(deviceID + " 👉 Thử bấm lại Sign Out...");
                        bool clickedAgain = ClickByDumpXml(deviceID, "Sign Out", 1, 500);
                        if (!clickedAgain)
                        {
                            KAutoHelper.ADBHelper.Tap(deviceID, 304, 2333);
                        }
                        await DelayWithPause(2000, token);
                    }
                }

                UpdateProcess(deviceID + " 🏁 Hoàn tất kiểm tra ExpressVPN.");
                GoHome(deviceID);
                await DelayWithPause(1000, token);
            }
            catch (Exception ex)
            {
                UpdateProcess(deviceID + " ⚠️ Lỗi trong SignOutExpressVpnAsync: " + ex.Message);
            }
        }

        private static readonly HttpClient changeDeviceHttpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(5)
        };

        double GetDeviceUptimeSeconds(string deviceID)
        {
            try
            {
                string uptimeStr = AdbShell(deviceID, "cat /proc/uptime");
                if (!string.IsNullOrWhiteSpace(uptimeStr))
                {
                    string firstPart = uptimeStr.Trim().Split(' ')[0];
                    if (double.TryParse(firstPart, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double upSec))
                    {
                        return upSec;
                    }
                }
            }
            catch { }
            return -1;
        }

        async Task<bool> ChangeDeviceAsync(string deviceID, CancellationToken token)
        {
            try
            {
                double uptimeBefore = GetDeviceUptimeSeconds(deviceID);
                UpdateProcess(deviceID + $" 🔄 Bắt đầu gọi API Change Device (US, Android 15)... (Uptime: {uptimeBefore:F1}s)");

                string url = $"http://localhost:9999/change?serial={deviceID}&filter_country=us&filter_os=15";

                using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token))
                {
                    linkedCts.CancelAfter(TimeSpan.FromMinutes(5));

                    HttpResponseMessage response = await changeDeviceHttpClient.GetAsync(url, linkedCts.Token);
                    string json = await response.Content.ReadAsStringAsync();

                    UpdateProcess(deviceID + $" 📥 Phản hồi API Change: {json.Trim()}");

                    bool isSuccess = false;
                    string message = "";
                    string processTime = "";

                    try
                    {
                        var jsonObj = Newtonsoft.Json.Linq.JObject.Parse(json);
                        isSuccess = (bool?)jsonObj["Success"] ?? false;
                        message = jsonObj["Message"]?.ToString() ?? "";
                        processTime = jsonObj["ProcessTime"]?.ToString() ?? "";
                    }
                    catch
                    {
                        isSuccess = json.Contains("\"Success\": true") || json.Contains("\"Success\":true");
                    }

                    if (isSuccess)
                    {
                        UpdateProcess(deviceID + $" 🎉 API báo Change Device thành công! ({message}, {processTime})");

                        // Kiểm tra trạng thái thiết bị sau khi change (đợi reboot & mạng sẵn sàng)
                        await WaitForDeviceReadyAsync(deviceID, uptimeBefore, token);
                        return true;
                    }
                    else
                    {
                        UpdateProcess(deviceID + $" ❌ API báo Change Device thất bại: {message}");
                        return false;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                if (token.IsCancellationRequested)
                    UpdateProcess(deviceID + " ⛔ Change device bị dừng theo yêu cầu hủy.");
                else
                    UpdateProcess(deviceID + " ⚠️ Quá thời gian chờ API Change Device (vượt quá 5 phút).");
                return false;
            }
            catch (Exception ex)
            {
                UpdateProcess(deviceID + " ⚠️ Lỗi trong ChangeDeviceAsync: " + ex.Message);
                return false;
            }
        }

        async Task WaitForDeviceReadyAsync(string deviceID, double uptimeBefore, CancellationToken token)
        {
            try
            {
                UpdateProcess(deviceID + " ⏳ Đang kiểm tra trạng thái thiết bị sau khi Change Device...");

                // 1. Chờ máy bắt đầu reboot (ngắt kết nối ADB hoặc uptime reset < 90s)
                for (int c = 0; c < 15; c++)
                {
                    if (token.IsCancellationRequested) return;
                    await Task.Delay(1000, token);

                    string state = AdbCommand($"-s {deviceID} get-state")?.Trim() ?? "";
                    double curUptime = GetDeviceUptimeSeconds(deviceID);

                    if (!state.Contains("device") || (curUptime >= 0 && curUptime < 90) || (curUptime >= 0 && uptimeBefore > 0 && curUptime < uptimeBefore - 10))
                    {
                        UpdateProcess(deviceID + " 🔄 Đã phát hiện thiết bị đang khởi động lại...");
                        break;
                    }
                }

                // 2. Chờ thiết bị boot hoàn tất (sys.boot_completed == "1")
                UpdateProcess(deviceID + " ⏳ Chờ thiết bị khởi động lại và nạp hệ thống hoàn chỉnh...");
                for (int i = 0; i < 60; i++)
                {
                    if (token.IsCancellationRequested) return;

                    string state = AdbCommand($"-s {deviceID} get-state")?.Trim() ?? "";
                    if (state.Contains("device"))
                    {
                        string boot = AdbShell(deviceID, "getprop sys.boot_completed")?.Trim() ?? "";
                        if (boot == "1")
                        {
                            string os = AdbShell(deviceID, "getprop ro.build.version.release")?.Trim() ?? "";
                            string model = AdbShell(deviceID, "getprop ro.product.model")?.Trim() ?? "";
                            string brand = AdbShell(deviceID, "getprop ro.product.brand")?.Trim() ?? "";

                            UpdateProcess(deviceID + $" ✅ Thiết bị đã boot xong! Brand: {brand}, Model: {model}, OS: Android {os}");
                            break;
                        }
                    }

                    await Task.Delay(2000, token);
                }

                // 3. Đánh thức và mở khóa màn hình
                AdbShell(deviceID, "input keyevent KEYCODE_WAKEUP");
                AdbShell(deviceID, "input keyevent 82");
                MuteDevice(deviceID);
                await Task.Delay(1500, token);

                // 4. BẮT BUỘC CHỜ WI-FI / INTERNET KẾT NỐI SẴN SÀNG
                UpdateProcess(deviceID + " 🌐 Kiểm tra kết nối mạng Wi-Fi sau khi khởi động lại...");
                bool networkReady = false;
                for (int net = 0; net < 25; net++)
                {
                    if (token.IsCancellationRequested) return;

                    string pingRes = AdbShell(deviceID, "ping -c 1 -W 2 8.8.8.8");
                    if (!string.IsNullOrWhiteSpace(pingRes) && (pingRes.Contains("1 received") || pingRes.Contains("0% packet loss")))
                    {
                        networkReady = true;
                        UpdateProcess(deviceID + " 🟢 Kết nối mạng Internet/Wi-Fi đã sẵn sàng 100%!");
                        break;
                    }

                    await Task.Delay(2000, token);
                }

                if (!networkReady)
                {
                    UpdateProcess(deviceID + " ⚠️ Cảnh báo: Chưa phát hiện Internet qua Ping sau 50s, chờ thêm 5s trước khi tiếp tục...");
                    await Task.Delay(5000, token);
                }
            }
            catch (Exception ex)
            {
                UpdateProcess(deviceID + " ⚠️ Lỗi trong WaitForDeviceReadyAsync: " + ex.Message);
            }
        }

        public static string Get2FA(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) return "";
            try
            {
                return Tiktok_helper.Get2FAKeyFromXml(xml);
            }
            catch
            {
                return "";
            }
        }
        public static string GetOTP(string secret)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(secret))
                    return "";

                secret = secret
                    .Replace(" ", "")
                    .Replace("-", "")
                    .Trim()
                    .ToUpper();

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
            catch
            {
                return "";
            }
        }
        public static byte[] Base32Decode(string base32)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

            if (string.IsNullOrWhiteSpace(base32))
                return new byte[0];

            base32 = base32
                .TrimEnd('=')
                .Replace(" ", "")
                .Replace("-", "")
                .ToUpper();

            List<byte> bytes = new List<byte>();

            int bitBuffer = 0;
            int bitCount = 0;

            foreach (char c in base32)
            {
                int value = alphabet.IndexOf(c);

                if (value < 0)
                    continue;

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

        readonly object lockImg = new object();

        ImgClone CloneImg()
        {
            lock (lockImg) // 🔥 tránh đụng nhau
            {
                return new ImgClone
                {
                    ggloin = (Bitmap)ggloin.Clone(),
                    bsn = (Bitmap)bsn.Clone(),
                    signin = (Bitmap)signin.Clone(),
                    iage = (Bitmap)iage.Clone(),
                    iage_new = (Bitmap)iage_new?.Clone(),
                    i_understand = (Bitmap)i_understand?.Clone(),
                    google_pw_error = (Bitmap)google_pw_error?.Clone(),
                    accept = (Bitmap)accept.Clone(),
                    skip2 = (Bitmap)skip2.Clone(),
                    hoso = (Bitmap)hoso.Clone(),
                    skipphone = (Bitmap)skipphone.Clone(),
                    google_error = (Bitmap)google_error?.Clone(),
                    create = (Bitmap)create.Clone(),
                    mat = (Bitmap)mat.Clone(),
                    security = (Bitmap)security.Clone(),
                    tich = (Bitmap)tich.Clone(),
                    st1 = (Bitmap)st1.Clone(),
                    verymail = (Bitmap)verymail.Clone(),
                    basoc = (Bitmap)basoc.Clone(),
                    google = (Bitmap)google.Clone(),
                    tieptuc = (Bitmap)tieptuc.Clone(),
                    capcha = (Bitmap)capcha.Clone(),
                    welcome = (Bitmap)welcome.Clone(),
                    ttlog = (Bitmap)ttlog.Clone(),
                    stepveri = (Bitmap)stepveri.Clone(),
                    securityy = (Bitmap)securityy.Clone(),
                    setinggiua = (Bitmap)setinggiua.Clone(),
                    setingtren = (Bitmap)setingtren.Clone(),
                    seting = (Bitmap)seting.Clone(),
                    seting1 = (Bitmap)seting1.Clone(),
                    phone = (Bitmap)phone.Clone(),
                    update = (Bitmap)update.Clone(),
                    capchalogin = (Bitmap)capchalogin.Clone(),
                    news = (Bitmap)news.Clone(),
                    pass_welcome = pass_welcome != null ? (Bitmap)pass_welcome.Clone() : null,
                    pass_show = pass_show != null ? (Bitmap)pass_show.Clone() : null,
                };
            }
        }
        public static string RandomPassword(int minLength = 9, int maxLength = 12)
        {
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string special = "@";

            int length = rd.Next(minLength, maxLength + 1);

            List<char> password = new List<char>
    {
        upper[rd.Next(upper.Length)],      // 1 chữ hoa
        digits[rd.Next(digits.Length)],    // 1 số
        special[rd.Next(special.Length)]   // 1 ký tự đặc biệt
    };

            string allChars = lower + upper + digits + special;

            for (int i = password.Count; i < length; i++)
            {
                password.Add(allChars[rd.Next(allChars.Length)]);
            }

            // trộn vị trí
            return new string(password.OrderBy(x => rd.Next()).ToArray());
        }
        private static readonly HttpClient _http = new HttpClient();
        public static async Task KhoUploader(string importUrl, string account)
        {
            if (string.IsNullOrWhiteSpace(account)) return;

            var payload = System.Text.Json.JsonSerializer.Serialize(new
            {
                items = new[] { account }
            });

            int retry = 0;
            int maxRetry = 5;

            while (retry < maxRetry)
            {
                try
                {
                    // tạo mới mỗi lần retry
                    var content = new StringContent(payload, Encoding.UTF8, "application/json");

                    var resp = await _http.PostAsync(importUrl, content);
                    var body = await resp.Content.ReadAsStringAsync();

                    var result = System.Text.Json.JsonSerializer
                        .Deserialize<System.Text.Json.JsonElement>(body);

                    if (result.GetProperty("success").GetBoolean())
                    {
                        Console.WriteLine($"[KHO] ✅ Đã upload: {account}");
                        return; // thành công → thoát luôn
                    }
                    else
                    {
                        retry++;
                        Console.WriteLine($"[KHO] ⚠️ Lần {retry}/{maxRetry} - API lỗi: {body}");
                    }
                }
                catch (Exception ex)
                {
                    retry++;
                    Console.WriteLine($"[KHO] ⚠️ Lần {retry}/{maxRetry} - Lỗi kết nối: {ex.Message}");
                }

                await Task.Delay(2000); // nghỉ 2s rồi thử lại
            }

            // nếu chạy hết 5 lần vẫn fail
            Console.WriteLine($"[KHO] ❌ Bỏ qua acc sau {maxRetry} lần thất bại: {account}");
        }

        private void txtExpressVpn_TextChanged(object sender, EventArgs e)
        {

        }

        public void UpdateDeviceLog(string devId, string status) => UpdateDeviceStatus(devId, status);

        public void UpdateDeviceStatus(string devId, string status)
        {
            if (dgvDevices == null || dgvDevices.IsDisposed) return;

            if (dgvDevices.InvokeRequired)
            {
                dgvDevices.BeginInvoke(new Action(() => UpdateDeviceStatus(devId, status)));
                return;
            }

            foreach (DataGridViewRow row in dgvDevices.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == devId)
                {
                    row.Cells[1].Value = status;
                    break;
                }
            }
        }

        void UpdateProcess(string text, string deviceID = null)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            string log = string.IsNullOrEmpty(deviceID)
                ? $"[{time}] {text}"
                : $"[{time}] [{deviceID}] {text}";

            try
            {
                File.AppendAllText("process_log.txt", log + Environment.NewLine);
            }
            catch { }

            if (processText.InvokeRequired)
            {
                processText.BeginInvoke(new Action(() =>
                {
                    processText.AppendText(log + Environment.NewLine);
                }));
            }
            else
            {
                processText.AppendText(log + Environment.NewLine);
            }

            // Cập nhật realtime vào cột Log của DataGridView cho device tương ứng
            try
            {
                if (!string.IsNullOrEmpty(deviceID))
                {
                    UpdateDeviceStatus(deviceID, text);
                }
                else if (dgvDevices != null)
                {
                    if (dgvDevices.InvokeRequired)
                    {
                        dgvDevices.BeginInvoke(new Action(() =>
                        {
                            foreach (DataGridViewRow row in dgvDevices.Rows)
                            {
                                string dev = row.Cells[0].Value?.ToString();
                                if (!string.IsNullOrEmpty(dev) && text.StartsWith(dev))
                                {
                                    string statusText = text.Substring(dev.Length).Trim();
                                    row.Cells[1].Value = statusText;
                                    break;
                                }
                            }
                        }));
                    }
                    else
                    {
                        foreach (DataGridViewRow row in dgvDevices.Rows)
                        {
                            string dev = row.Cells[0].Value?.ToString();
                            if (!string.IsNullOrEmpty(dev) && text.StartsWith(dev))
                            {
                                string statusText = text.Substring(dev.Length).Trim();
                                row.Cells[1].Value = statusText;
                                break;
                            }
                        }
                    }
                }
            }
            catch { }
        }

        void StartDevice(string deviceID, int index)
        {
            lock (deviceLock)
            {
                if (deviceTokens.ContainsKey(deviceID))
                {
                    UpdateProcess(deviceID + " ⚠️ Đang chạy → bỏ qua");
                    return;
                }

                var cts = new CancellationTokenSource();
                deviceTokens[deviceID] = cts;
                deviceStartTime[deviceID] = DateTime.Now;

                var pauseEvt = devicePauseEvents.GetOrAdd(deviceID, _ => new ManualResetEventSlim(true));
                pauseEvt.Set();
                devicePausedStatus[deviceID] = false;

                UpdateProcess(deviceID + " 🚀 StartDevice");
                Task.Run(() => RunDevice(deviceID, index, cts.Token));
            }
        }
        void LoadDevices()
        {
            if (dgvDevices.InvokeRequired)
            {
                dgvDevices.Invoke(new Action(LoadDevices));
                return;
            }

            dgvDevices.Rows.Clear();

            var devices = KAutoHelper.ADBHelper.GetDevices();

            foreach (var d in devices)
            {
                dgvDevices.Rows.Add(d, "Sẵn sàng");
            }
        }
        private static readonly object fileLock = new object();

        public static void SaveAccount(string usernamedata)
        {
            string path = "accounts.txt";
            string line = $"{usernamedata}";

            lock (fileLock)
            {
                System.IO.File.AppendAllText(path, line + Environment.NewLine);
            }
        }

        public static void SaveCreatedAccount(string usernamedata)
        {
            string path = "created_accounts.txt";
            string line = $"{usernamedata}";

            lock (fileLock)
            {
                System.IO.File.AppendAllText(path, line + Environment.NewLine);
            }
        }

        public static void SaveAccountFullBackup(string email, string passMail, string username, string passTiktok, string twoFa, string unlink)
        {
            try
            {
                string fullAcc = $"{username}|{passTiktok}|{twoFa}";
                string line = $"{email}|{passMail}|{username}|{passTiktok}|{twoFa}|{fullAcc}|{unlink}|{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                lock (fileLock)
                {
                    File.AppendAllText("accounts_full.txt", line + Environment.NewLine);
                }
            }
            catch { }
        }

        private static readonly HttpClient _sheetHttpClient = new HttpClient(new HttpClientHandler
        {
            AllowAutoRedirect = true
        })
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        public async Task<bool> SendToGoogleSheetAsync(string email, string passMail, string username, string passTiktok, string twoFa, string unlink)
        {
            string url = "";
            if (txtGoogleSheetUrl.InvokeRequired)
            {
                url = (string)txtGoogleSheetUrl.Invoke(new Func<string>(() => txtGoogleSheetUrl.Text.Trim()));
            }
            else
            {
                url = txtGoogleSheetUrl.Text.Trim();
            }

            // Luôn lưu bản backup đầy đủ cục bộ vào file accounts_full.txt
            SaveAccountFullBackup(email, passMail, username, passTiktok, twoFa, unlink);

            if (string.IsNullOrWhiteSpace(url))
            {
                return false;
            }

            try
            {
                var payload = new
                {
                    email = email ?? "",
                    passMail = passMail ?? "",
                    username = username ?? "",
                    passTiktok = passTiktok ?? "",
                    twoFa = twoFa ?? "",
                    fullAcc = $"{username}|{passTiktok}|{twoFa}",
                    unlink = unlink ?? "",
                    createdAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    var response = await _sheetHttpClient.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateProcess($"⚠️ Lỗi khi gửi dữ liệu lên Google Sheet: {ex.Message}");
            }
            return false;
        }

        private async void btnTestGoogleSheet_Click(object sender, EventArgs e)
        {
            string url = txtGoogleSheetUrl.Text.Trim();
            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("Vui lòng dán link Google Apps Script Webhook URL vào ô trước khi gửi thử!", "Chưa có URL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnTestGoogleSheet.Enabled = false;
            lblSheetTestStatus.Text = "⏳ Đang gửi dòng test lên Google Sheet...";
            lblSheetTestStatus.ForeColor = Color.FromArgb(59, 130, 246);

            try
            {
                bool ok = await SendToGoogleSheetAsync("test_email@gmail.com", "pass123456", "test_tiktok_user", "Cocasocola@1", "JBSWY3DPEHPK3PXP", "ok");
                if (ok)
                {
                    lblSheetTestStatus.Text = "✅ Gửi test thành công! Hãy mở Google Sheet xem dòng mới.";
                    lblSheetTestStatus.ForeColor = Color.FromArgb(16, 185, 129);
                    try { File.WriteAllText("googlesheet_url.txt", url); } catch { }
                }
                else
                {
                    lblSheetTestStatus.Text = "❌ Gửi test thất bại! Hãy kiểm tra lại URL Webhook hoặc quyền Deploy.";
                    lblSheetTestStatus.ForeColor = Color.FromArgb(239, 68, 68);
                }
            }
            catch (Exception ex)
            {
                lblSheetTestStatus.Text = "❌ Lỗi: " + ex.Message;
                lblSheetTestStatus.ForeColor = Color.FromArgb(239, 68, 68);
            }
            finally
            {
                btnTestGoogleSheet.Enabled = true;
            }
        }

        private void btnSaveSheetUrl_Click(object sender, EventArgs e)
        {
            try
            {
                string url = txtGoogleSheetUrl.Text.Trim();
                File.WriteAllText("googlesheet_url.txt", url);
                lblSheetTestStatus.Text = "💾 Đã lưu Google Sheet URL thành công!";
                lblSheetTestStatus.ForeColor = Color.FromArgb(16, 185, 129);
            }
            catch (Exception ex)
            {
                lblSheetTestStatus.Text = "❌ Không thể lưu file: " + ex.Message;
                lblSheetTestStatus.ForeColor = Color.FromArgb(239, 68, 68);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadDevices();
            InitGmailVipControls();
            if (File.Exists("vpn_provider.txt"))
            {
                try
                {
                    string vpnType = File.ReadAllText("vpn_provider.txt").Trim();
                    if (vpnType == "HMA VPN" || vpnType == "ExpressVPN")
                    {
                        cboVpnProvider.SelectedItem = vpnType;
                    }
                }
                catch { }
            }
            if (cboVpnProvider.SelectedIndex < 0)
            {
                cboVpnProvider.SelectedIndex = 0;
            }

            if (File.Exists("expressvpn.txt"))
            {
                try
                {
                    txtExpressVpn.Text = File.ReadAllText("expressvpn.txt").Trim();
                }
                catch { }
            }

            if (File.Exists("googlesheet_url.txt"))
            {
                try
                {
                    txtGoogleSheetUrl.Text = File.ReadAllText("googlesheet_url.txt").Trim();
                    if (!string.IsNullOrWhiteSpace(txtGoogleSheetUrl.Text))
                    {
                        lblSheetTestStatus.Text = "🟢 Đã tải Webhook URL từ file cấu hình.";
                        lblSheetTestStatus.ForeColor = Color.FromArgb(16, 185, 129);
                    }
                }
                catch { }
            }

            if (File.Exists("emails.txt") && string.IsNullOrWhiteSpace(txtEmails.Text))
            {
                try
                {
                    txtEmails.Text = File.ReadAllText("emails.txt");
                    selectedEmailFilePath = "emails.txt";
                    labelEmails.Text = "DS Email (emails.txt):";
                    LoadEmailQueue();
                }
                catch { }
            }

            this.Shown += (s, ev) =>
            {
                if (Environment.GetCommandLineArgs().Contains("--autostart"))
                {
                    button1.PerformClick();
                }
            };

            // Hiệu ứng hover mượt mà phong cách Fluent UI
            SetupButtonHover(button1, Color.FromArgb(16, 185, 129), Color.FromArgb(5, 150, 105));
            SetupButtonHover(btnStop, Color.FromArgb(245, 158, 11), Color.FromArgb(217, 119, 6));
            SetupButtonHover(btnStopAll, Color.FromArgb(239, 68, 68), Color.FromArgb(220, 38, 38));
            SetupButtonHover(btnContinue, Color.FromArgb(59, 130, 246), Color.FromArgb(37, 99, 235));
            SetupButtonHover(btnSelectEmailFile, Color.FromArgb(238, 242, 255), Color.FromArgb(224, 231, 255));
            SetupButtonHover(btnReset, Color.FromArgb(241, 245, 249), Color.FromArgb(226, 232, 240));
            SetupButtonHover(btnReloadDevices, Color.White, Color.FromArgb(241, 245, 249));
            SetupButtonHover(btnTestGoogleSheet, Color.FromArgb(238, 242, 255), Color.FromArgb(224, 231, 255));
            SetupButtonHover(btnSaveSheetUrl, Color.FromArgb(241, 245, 249), Color.FromArgb(226, 232, 240));

            txtEmails.AllowDrop = true;
            txtEmails.DragEnter += (s, ev) =>
            {
                if (ev.Data.GetDataPresent(DataFormats.FileDrop))
                    ev.Effect = DragDropEffects.Copy;
            };
            txtEmails.DragDrop += (s, ev) =>
            {
                try
                {
                    string[] files = (string[])ev.Data.GetData(DataFormats.FileDrop);
                    if (files != null && files.Length > 0 && File.Exists(files[0]))
                    {
                        selectedEmailFilePath = files[0];
                        txtEmails.Text = File.ReadAllText(selectedEmailFilePath);
                        labelEmails.Text = $"DS Email ({Path.GetFileName(selectedEmailFilePath)}):";
                        LoadEmailQueue();
                        UpdateProcess($"📂 Đã nạp file email: {Path.GetFileName(selectedEmailFilePath)} ({emailQueue.Count} email)");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kéo thả file: " + ex.Message);
                }
            };
        }

        private void SetupButtonHover(Button btn, Color normalColor, Color hoverColor)
        {
            if (btn == null) return;
            btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = normalColor;
        }
        public static async Task<string> GetDeviceIPAsync(string deviceID)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    string result = AdbShell(deviceID, "curl -s https://api.ipify.org");
                    if (!string.IsNullOrWhiteSpace(result) && result.Contains("."))
                        return result.Trim();

                    result = AdbShell(deviceID, "curl -s ifconfig.me");

                    if (!string.IsNullOrWhiteSpace(result) && result.Contains("."))
                        return result.Trim();

                    result = AdbShell(deviceID, "wget -qO- ifconfig.me");

                    if (!string.IsNullOrWhiteSpace(result) && result.Contains("."))
                        return result.Trim();

                    result = AdbShell(deviceID, "toybox wget -qO- ifconfig.me");

                    if (!string.IsNullOrWhiteSpace(result) && result.Contains("."))
                        return result.Trim();
                }
                catch { }

                await Task.Delay(2000);
            }

            return null;
        }
        public class EmailAccount
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public static async Task<EmailAccount> GenerateEmailAsync(int retry = 10)
        {
            string url = "https://mail.giaphoangtn.io.vn/api/email/generate?key=a89b6f2b98d62a06aca09e26e2271b71c3035b30c809b3d8";

            using (HttpClient client = new HttpClient())
            {
                for (int i = 1; i <= retry; i++)
                {
                    await mailRequestLock.WaitAsync();

                    try
                    {
                        string result = await client.GetStringAsync(url);

                        if (string.IsNullOrWhiteSpace(result) || !result.Contains("|"))
                        {
                            Console.WriteLine("[MAIL] ❌ Response lỗi: " + result);
                            continue;
                        }

                        var parts = result.Split('|');

                        string email = parts[0].Trim();
                        string password = parts[1].Trim();

                        if (!usedMail.Contains(email))
                        {
                            usedMail.Add(email);

                            Console.WriteLine("[MAIL] ✅ Mail mới: " + email);

                            return new EmailAccount
                            {
                                Email = email,
                                Password = password
                            };
                        }

                        Console.WriteLine("[MAIL] ⚠️ Mail trùng, gọi lại: " + email);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[MAIL] ❌ Lỗi: " + ex.Message);
                    }
                    finally
                    {
                        mailRequestLock.Release();
                    }

                    await Task.Delay(1000);
                }
            }

            return null;
        }
        public static async Task<string> GetMailCodeAsync(string emailWithPass, int retry = 10)
        {
            string url = "https://mail.giaphoangtn.io.vn/api/email/latest?email="
                         + Uri.EscapeDataString(emailWithPass);

            using (HttpClient client = new HttpClient())
            {
                for (int i = 1; i <= retry; i++)
                {
                    try
                    {
                        string content = await client.GetStringAsync(url);

                        if (!string.IsNullOrWhiteSpace(content))
                        {
                            var match = System.Text.RegularExpressions.Regex.Match(
                                content,
                                @"\b\d{6}\b"
                            );

                            if (match.Success)
                            {
                                Console.WriteLine("[MAIL] ✅ Code: " + match.Value);
                                return match.Value;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[MAIL] ❌ Lỗi: " + ex.Message);
                    }

                    Console.WriteLine($"[MAIL] ⏳ Chưa có code... thử {i}/{retry}");
                    await Task.Delay(5000);
                }
            }

            return null;
        }
        public static bool ClickByTextIndexParent(string deviceID, string text, int index = 0, int timeout = 10000)
        {
            Stopwatch sw = Stopwatch.StartNew();

            while (sw.ElapsedMilliseconds < timeout)
            {
                string xml = Android.GetUIDumpSafe(deviceID);

                if (string.IsNullOrWhiteSpace(xml))
                {
                    Thread.Sleep(300);
                    continue;
                }

                if (TryGetClickableParentPoint(xml, text, index, out int x, out int y))
                {
                    Console.WriteLine($"Click {text} index={index} tại x={x}, y={y}");
                    KAutoHelper.ADBHelper.Tap(deviceID, x, y);
                    return true;
                }

                Thread.Sleep(300);
            }

            return false;
        }
        public static bool TryGetClickableParentPoint(string xml, string text, int index, out int x, out int y)
        {
            x = 0;
            y = 0;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                var nodes = doc.SelectNodes($"//node[@text='{text}' or @content-desc='{text}']")
                    .Cast<XmlNode>()
                    .Select(n => new
                    {
                        Node = n,
                        Bounds = n.Attributes["bounds"]?.Value
                    })
                    .Where(n => !string.IsNullOrWhiteSpace(n.Bounds))
                    .OrderBy(n => GetY1(n.Bounds))
                    .ToList();

                if (nodes.Count <= index)
                    return false;

                XmlNode node = nodes[index].Node;

                // Nếu chính nó clickable=true thì dùng luôn, nếu không thì leo lên node cha
                if (node.Attributes?["clickable"]?.Value != "true")
                {
                    XmlNode current = node;
                    while (current != null && current.Attributes?["clickable"]?.Value != "true")
                    {
                        current = current.ParentNode;
                    }
                    if (current != null)
                    {
                        node = current;
                    }
                }

                string bounds = node.Attributes["bounds"]?.Value;
                if (string.IsNullOrWhiteSpace(bounds))
                    return false;

                return GetCenter(bounds, out x, out y);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi click text parent: " + ex.Message);
                return false;
            }
        }
        public static int GetY1(string bounds)
        {
            var match = Regex.Match(bounds, @"\[(\d+),(\d+)\]\[(\d+),(\d+)\]");
            if (!match.Success) return 0;

            return int.Parse(match.Groups[2].Value);
        }
        public static bool GetCenter(string bounds, out int x, out int y)
        {
            x = 0;
            y = 0;

            var match = Regex.Match(bounds, @"\[(\d+),(\d+)\]\[(\d+),(\d+)\]");
            if (!match.Success) return false;

            int x1 = int.Parse(match.Groups[1].Value);
            int y1 = int.Parse(match.Groups[2].Value);
            int x2 = int.Parse(match.Groups[3].Value);
            int y2 = int.Parse(match.Groups[4].Value);

            if (x1 >= x2 || y1 >= y2 || (x1 == 0 && x2 == 0 && y1 == 0 && y2 == 0))
                return false;

            x = (x1 + x2) / 2;
            y = (y1 + y2) / 2;

            return true;
        }

        public static bool GetBounds(string bounds, out int x1, out int y1, out int x2, out int y2)
        {
            x1 = y1 = x2 = y2 = 0;
            var match = Regex.Match(bounds ?? "", @"\[(\d+),(\d+)\]\[(\d+),(\d+)\]");
            if (!match.Success) return false;

            x1 = int.Parse(match.Groups[1].Value);
            y1 = int.Parse(match.Groups[2].Value);
            x2 = int.Parse(match.Groups[3].Value);
            y2 = int.Parse(match.Groups[4].Value);

            return (x2 > x1 && y2 > y1);
        }

        public static bool ClickAccountInSettings(string deviceID)
        {
            try
            {
                string xml = Android.GetUIDumpSafe(deviceID);
                if (!string.IsNullOrWhiteSpace(xml))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);

                    var nodes = doc.SelectNodes("//node[@text='Account' or @content-desc='Account']");
                    if (nodes != null)
                    {
                        foreach (XmlNode node in nodes)
                        {
                            string bounds = node.Attributes?["bounds"]?.Value;
                            if (GetBounds(bounds, out int x1, out int y1, out int x2, out int y2))
                            {
                                if ((x2 - x1) > 500)
                                {
                                    KAutoHelper.ADBHelper.Tap(deviceID, (x1 + x2) / 2, (y1 + y2) / 2);
                                    return true;
                                }
                            }
                        }

                        foreach (XmlNode node in nodes)
                        {
                            XmlNode parent = node.ParentNode;
                            if (parent != null)
                            {
                                string bounds = parent.Attributes?["bounds"]?.Value;
                                if (GetBounds(bounds, out int x1, out int y1, out int x2, out int y2))
                                {
                                    if ((x2 - x1) > 500)
                                    {
                                        KAutoHelper.ADBHelper.Tap(deviceID, (x1 + x2) / 2, (y1 + y2) / 2);
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ClickAccountInSettings error: " + ex.Message);
            }

            // Fallback tọa độ
            KAutoHelper.ADBHelper.Tap(deviceID, 720, 1992);
            return true;
        }

        public class CityLocation
        {
            public string Name { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }

        public static List<CityLocation> ExtractUsaCities(string xml)
        {
            var list = new List<CityLocation>();
            if (string.IsNullOrWhiteSpace(xml)) return list;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                var nodes = doc.SelectNodes("//node[starts-with(@text, 'USA - ')]");
                if (nodes != null)
                {
                    foreach (XmlNode n in nodes)
                    {
                        string text = n.Attributes?["text"]?.Value;
                        string bounds = n.Attributes?["bounds"]?.Value;
                        if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(bounds))
                        {
                            if (GetCenter(bounds, out int cx, out int cy))
                            {
                                list.Add(new CityLocation { Name = text, X = cx, Y = cy });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi trích xuất danh sách thành phố US: " + ex.Message);
            }
            return list;
        }

        public static Point? GetExpandButtonForCountry(string xml, string countryName)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                var node = doc.SelectSingleNode($"//node[@text='{countryName}']");
                if (node != null)
                {
                    XmlNode parent = node.ParentNode;
                    while (parent != null && parent.Attributes?["clickable"]?.Value != "true")
                    {
                        parent = parent.ParentNode;
                    }
                    if (parent != null)
                    {
                        var btn = parent.SelectSingleNode(".//node[@class='android.widget.Button']");
                        if (btn != null)
                        {
                            string bounds = btn.Attributes?["bounds"]?.Value;
                            if (GetCenter(bounds, out int bx, out int by))
                                return new Point(bx, by);
                        }

                        string parentBounds = parent.Attributes?["bounds"]?.Value;
                        var match = Regex.Match(parentBounds, @"\[(\d+),(\d+)\]\[(\d+),(\d+)\]");
                        if (match.Success)
                        {
                            int x2 = int.Parse(match.Groups[3].Value);
                            int y1 = int.Parse(match.Groups[2].Value);
                            int y2 = int.Parse(match.Groups[4].Value);
                            return new Point(x2 - 100, (y1 + y2) / 2);
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        public static bool ClickByResourceId(string deviceID, string resourceId, string xml = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(xml))
                    xml = Android.GetUIDumpSafe(deviceID);
                if (string.IsNullOrWhiteSpace(xml)) return false;

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                var node = doc.SelectSingleNode($"//node[@resource-id='{resourceId}' or contains(@resource-id, '{resourceId}')]");
                if (node != null)
                {
                    string bounds = node.Attributes?["bounds"]?.Value;
                    if (GetCenter(bounds, out int cx, out int cy))
                    {
                        KAutoHelper.ADBHelper.Tap(deviceID, cx, cy);
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }

        public static void ClearFieldAt(string deviceID, int x, int y, int count = 40)
        {
            try
            {
                KAutoHelper.ADBHelper.Tap(deviceID, x, y);
                Thread.Sleep(150);
                string delKeys = string.Join(" ", Enumerable.Repeat("67", count));
                AdbShell(deviceID, $"input keyevent 123 {delKeys}");
                Thread.Sleep(150);
            }
            catch { }
        }

        public static void FastInputText(string deviceID, string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text)) return;
                var sb = new StringBuilder();
                foreach (char c in text)
                {
                    if (c == ' ')
                    {
                        sb.Append("%s");
                    }
                    else if (c == '\\' || c == '&' || c == '<' || c == '>' || c == '(' || c == ')' ||
                             c == ';' || c == '\'' || c == '"' || c == '`' || c == '$' || c == '|')
                    {
                        sb.Append('\\');
                        sb.Append(c);
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                AdbShell(deviceID, "input text " + sb.ToString());
            }
            catch { }
        }

        public static bool EnterExpressVpnCredentials(string deviceID, string vpnEmail, string vpnPass)
        {
            try
            {
                string xml = Android.GetUIDumpSafe(deviceID);

                int emailX = 720, emailY = 1378;
                int passX = 720, passY = 1669;
                int submitX = 720, submitY = 2067;
                bool hasSubmitNode = false;

                if (!string.IsNullOrWhiteSpace(xml))
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(xml);

                        // 1. Tìm trường Email
                        var emailNode = doc.SelectSingleNode("//node[@resource-id='signin_email_field' or contains(@resource-id, 'email') or (@class='android.widget.EditText' and not(@password='true'))]");
                        if (emailNode != null)
                        {
                            string emailBounds = emailNode.Attributes?["bounds"]?.Value;
                            GetCenter(emailBounds, out emailX, out emailY);
                        }

                        // 2. Tìm trường Password
                        var passNode = doc.SelectSingleNode("//node[@resource-id='signin_password_field' or contains(@resource-id, 'password') or (@class='android.widget.EditText' and @password='true')]");
                        if (passNode != null)
                        {
                            string passBounds = passNode.Attributes?["bounds"]?.Value;
                            GetCenter(passBounds, out passX, out passY);
                        }

                        // 3. Tìm nút Submit Sign In
                        var submitNode = doc.SelectSingleNode("//node[@resource-id='signin_submit_button' or (contains(@class, 'Button') and contains(@text, 'Sign In'))]");
                        if (submitNode != null)
                        {
                            string submitBounds = submitNode.Attributes?["bounds"]?.Value;
                            if (GetCenter(submitBounds, out submitX, out submitY))
                            {
                                hasSubmitNode = true;
                            }
                        }
                    }
                    catch { }
                }

                // 1. Nhập email: tap, xóa nhanh và paste thẳng vào ô email
                ClearFieldAt(deviceID, emailX, emailY);
                FastInputText(deviceID, vpnEmail);
                Thread.Sleep(300);

                // 2. Nhập password: tap, xóa nhanh và paste thẳng vào ô password
                ClearFieldAt(deviceID, passX, passY);
                FastInputText(deviceID, vpnPass);
                Thread.Sleep(400);

                // 3. BẮT BUỘC ĐÓNG BÀN PHÍM ẢO TRƯỚC KHI BẤM NÚT SIGN IN
                AdbShell(deviceID, "input keyevent 4");
                Thread.Sleep(500);

                // 4. Bấm nút Sign In (dùng tọa độ tìm thấy hoặc fallback 720, 2067)
                if (hasSubmitNode)
                {
                    KAutoHelper.ADBHelper.Tap(deviceID, submitX, submitY);
                }
                else
                {
                    KAutoHelper.ADBHelper.Tap(deviceID, 720, 2067);
                }
                Thread.Sleep(1500);

                // Kiểm tra nếu vẫn còn ở form Sign In (do bàn phím mở lại hoặc bấm trượt)
                string postXml = Android.GetUIDumpSafe(deviceID);
                if (!string.IsNullOrWhiteSpace(postXml) && (postXml.Contains("signin_email_field") || postXml.Contains("signin_password_field")))
                {
                    AdbShell(deviceID, "input keyevent 4");
                    Thread.Sleep(400);
                    if (hasSubmitNode)
                    {
                        KAutoHelper.ADBHelper.Tap(deviceID, submitX, submitY);
                    }
                    else
                    {
                        KAutoHelper.ADBHelper.Tap(deviceID, 720, 2067);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("EnterExpressVpnCredentials error: " + ex.Message);
            }
            return false;
        }

        public static bool ClickByDumpXml(string deviceID, string textToFind, int retries = 3, int delayMs = 1200)
        {
            string lower = textToFind.Trim().ToLower();
            for (int r = 0; r < retries; r++)
            {
                try
                {
                    string xml = Android.GetUIDumpSafe(deviceID);
                    if (!string.IsNullOrWhiteSpace(xml))
                    {
                        if (ClickByCaseInsensitiveText(deviceID, xml, textToFind))
                        {
                            return true;
                        }

                        // Fallback Regex
                        var matches = Regex.Matches(xml, @"<node[^>]+?>");
                        foreach (Match m in matches)
                        {
                            string tag = m.Value;
                            var mBounds = Regex.Match(tag, @"bounds=""\[(\d+),(\d+)\]\[(\d+),(\d+)\]""");
                            if (!mBounds.Success) continue;

                            var mText = Regex.Match(tag, @"text=""([^""]*)""");
                            var mDesc = Regex.Match(tag, @"content-desc=""([^""]*)""");

                            string tVal = mText.Success ? WebUtility.HtmlDecode(mText.Groups[1].Value).Trim().ToLower() : "";
                            string dVal = mDesc.Success ? WebUtility.HtmlDecode(mDesc.Groups[1].Value).Trim().ToLower() : "";

                            if (tVal == lower || dVal == lower ||
                                (!string.IsNullOrEmpty(tVal) && tVal.Contains(lower)) ||
                                (!string.IsNullOrEmpty(dVal) && dVal.Contains(lower)))
                            {
                                int x1 = int.Parse(mBounds.Groups[1].Value);
                                int y1 = int.Parse(mBounds.Groups[2].Value);
                                int x2 = int.Parse(mBounds.Groups[3].Value);
                                int y2 = int.Parse(mBounds.Groups[4].Value);
                                if (x2 > x1 && y2 > y1)
                                {
                                    int cx = (x1 + x2) / 2;
                                    int cy = (y1 + y2) / 2;
                                    KAutoHelper.ADBHelper.Tap(deviceID, cx, cy);
                                    return true;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ClickByDumpXml error: " + ex.Message);
                }

                if (r < retries - 1)
                {
                    Thread.Sleep(delayMs);
                }
            }
            return false;
        }

        public static bool ClickByCaseInsensitiveText(string deviceID, string xml, string textToFind)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(xml))
                    xml = Android.GetUIDumpSafe(deviceID);
                if (string.IsNullOrWhiteSpace(xml)) return false;

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                string lower = textToFind.Trim().ToLower();
                var nodes = doc.SelectNodes("//node[@text or @content-desc]");
                if (nodes != null)
                {
                    // Ưu tiên 1: Khớp chính xác hoàn toàn (exact match)
                    foreach (XmlNode node in nodes)
                    {
                        string txt = (node.Attributes?["text"]?.Value ?? "").Trim().ToLower();
                        string desc = (node.Attributes?["content-desc"]?.Value ?? "").Trim().ToLower();

                        if (txt == lower || desc == lower)
                        {
                            string bounds = node.Attributes?["bounds"]?.Value;
                            if (GetCenter(bounds, out int cx, out int cy))
                            {
                                KAutoHelper.ADBHelper.Tap(deviceID, cx, cy);
                                return true;
                            }
                        }
                    }

                    // Ưu tiên 2: Nút bấm (Button/clickable) có chứa text
                    foreach (XmlNode node in nodes)
                    {
                        string cls = node.Attributes?["class"]?.Value ?? "";
                        string clickable = node.Attributes?["clickable"]?.Value ?? "";
                        if (cls.Contains("Button") || clickable == "true")
                        {
                            string txt = (node.Attributes?["text"]?.Value ?? "").Trim().ToLower();
                            string desc = (node.Attributes?["content-desc"]?.Value ?? "").Trim().ToLower();

                            // Tránh click nhầm link trợ giúp "Learn more" / "Tìm hiểu thêm"
                            if ((lower == "more" || lower == "thêm") &&
                                (txt.Contains("learn more") || desc.Contains("learn more") ||
                                 txt.Contains("tìm hiểu thêm") || desc.Contains("tìm hiểu thêm")))
                            {
                                continue;
                            }

                            if (txt.Contains(lower) || desc.Contains(lower))
                            {
                                string bounds = node.Attributes?["bounds"]?.Value;
                                if (GetCenter(bounds, out int cx, out int cy))
                                {
                                    KAutoHelper.ADBHelper.Tap(deviceID, cx, cy);
                                    return true;
                                }
                            }
                        }
                    }

                    // Ưu tiên 3: Fallback chứa text nhưng không phải đoạn văn dài (> 50 ký tự)
                    foreach (XmlNode node in nodes)
                    {
                        string txt = (node.Attributes?["text"]?.Value ?? "").Trim();
                        string desc = (node.Attributes?["content-desc"]?.Value ?? "").Trim();
                        string txtLower = txt.ToLower();
                        string descLower = desc.ToLower();

                        // Tránh click nhầm link trợ giúp "Learn more" / "Tìm hiểu thêm"
                        if ((lower == "more" || lower == "thêm") &&
                            (txtLower.Contains("learn more") || descLower.Contains("learn more") ||
                             txtLower.Contains("tìm hiểu thêm") || descLower.Contains("tìm hiểu thêm")))
                        {
                            continue;
                        }

                        if ((txt.Length <= 50 && txtLower.Contains(lower)) ||
                            (desc.Length <= 50 && descLower.Contains(lower)))
                        {
                            string bounds = node.Attributes?["bounds"]?.Value;
                            if (GetCenter(bounds, out int cx, out int cy))
                            {
                                KAutoHelper.ADBHelper.Tap(deviceID, cx, cy);
                                return true;
                            }
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        public static void ClickFirstEditText(string deviceID, string xml)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(xml)) return;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                var node = doc.SelectSingleNode("//node[@class='android.widget.EditText']");
                if (node != null)
                {
                    string bounds = node.Attributes?["bounds"]?.Value;
                    if (GetCenter(bounds, out int cx, out int cy))
                    {
                        KAutoHelper.ADBHelper.Tap(deviceID, cx, cy);
                    }
                }
            }
            catch { }
        }

        // khóa xoay màn hình và khóa chiều dọc
        public static async Task<bool> LockPortraitAsync(string deviceID, int retry = 3)
        {
            if (string.IsNullOrWhiteSpace(deviceID))
                return false;

            for (int i = 1; i <= retry; i++)
            {
                try
                {
                    // Tắt xoay màn hình tự động
                    AdbShell(deviceID,
                        "settings put system accelerometer_rotation 0");

                    await Task.Delay(500);

                    // Khóa hướng dọc
                    AdbShell(deviceID,
                        "settings put system user_rotation 0");

                    await Task.Delay(500);

                    Console.WriteLine(
                        $"[ROTATE] ✅ {deviceID} đã khóa màn hình dọc");

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"[ROTATE] ❌ Lỗi lần {i}: {ex.Message}");

                    await Task.Delay(1000);
                }
            }

            return false;
        }
        public static async Task<bool> ForceLockPortraitAsync(string deviceID, int retry = 5)
        {
            if (string.IsNullOrWhiteSpace(deviceID))
                return false;

            for (int i = 1; i <= retry; i++)
            {
                // 1. Tắt auto rotate
                AdbShell(deviceID, "settings put system accelerometer_rotation 0");

                // 2. Ép hướng dọc
                AdbShell(deviceID, "settings put system user_rotation 0");

                // 3. Một số ROM dùng key này
                AdbShell(deviceID, "settings put secure show_rotation_suggestions 0");

                // 4. Refresh nhẹ màn hình
                AdbShell(deviceID, "cmd window dismiss-keyguard");
                AdbShell(deviceID, "input keyevent KEYCODE_WAKEUP");
                MuteDevice(deviceID);

                await Task.Delay(1000);

                string autoRotate = AdbShell(deviceID, "settings get system accelerometer_rotation")?.Trim();
                string rotation = AdbShell(deviceID, "settings get system user_rotation")?.Trim();

                if (autoRotate == "0" && rotation == "0")
                {
                    Console.WriteLine($"[ROTATE] ✅ {deviceID} đã khóa dọc OK");
                    return true;
                }

                Console.WriteLine($"[ROTATE] ⚠️ {deviceID} chưa ăn lệnh, thử lại {i}/{retry}");
                await Task.Delay(1000);
            }

            Console.WriteLine($"[ROTATE] ❌ {deviceID} khóa dọc thất bại");
            return false;
        }

        #endregion
    }

}

class ImgClone : IDisposable
{
    public Bitmap ggloin, bsn, signin, update, iage, iage_new, i_understand, google_pw_error, capchalogin, accept, skip2, news, stepveri, phone,
                  hoso, skipphone, google_error, create, mat, security, seting1, securityy,
                  tich, st1, verymail, basoc, google, tieptuc, capcha,
                  welcome, setinggiua, setingtren, seting, ttlog, pass_welcome, pass_show;

    public void Dispose()
    {
        ggloin?.Dispose();
        bsn?.Dispose();
        signin?.Dispose();
        update?.Dispose();
        iage?.Dispose();
        iage_new?.Dispose();
        i_understand?.Dispose();
        google_pw_error?.Dispose();
        capchalogin?.Dispose();
        accept?.Dispose();
        skip2?.Dispose();
        stepveri?.Dispose();
        phone?.Dispose();
        hoso?.Dispose();
        skipphone?.Dispose();
        google_error?.Dispose();
        create?.Dispose();
        mat?.Dispose();
        security?.Dispose();
        seting1?.Dispose();
        securityy?.Dispose();
        tich?.Dispose();
        st1?.Dispose();
        verymail?.Dispose();
        basoc?.Dispose();
        google?.Dispose();
        tieptuc?.Dispose();
        capcha?.Dispose();
        welcome?.Dispose();
        setinggiua?.Dispose();
        setingtren?.Dispose();
        seting?.Dispose();
        ttlog?.Dispose();
        pass_welcome?.Dispose();
        pass_show?.Dispose();
    }
}

public class ApiResponse
{
    public string Name { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
    public string ProcessTime { get; set; }
}


