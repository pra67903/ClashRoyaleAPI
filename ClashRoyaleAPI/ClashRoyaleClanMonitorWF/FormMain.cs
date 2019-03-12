﻿using ClashRoyaleAPI;
using ClashRoyaleAPI.BL;
using ClashRoyaleAPI.Exceptions;
using ClashRoyaleAPI.Models.Cards;
using ClashRoyaleAPI.Models.Clans;
using ClashRoyaleAPI.Models.Players;
using ClashRoyaleClanMonitorWF.Controls.Clans;
using ClashRoyaleClanMonitorWF.Controls.Main;
using ClashRoyaleClanMonitorWF.Controls.Settings;
using ClashRoyaleClanMonitorWF.Utils;
using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClashRoyaleClanMonitorWF
{
    public partial class FormMain : MetroForm
    {
        SettingsUC settingsUC = new SettingsUC();
        ClanCoreUC clanCoreUC;
        MainUC mainUC;
        Players player = new Players();
        Clans clan = new Clans();
        
        public FormMain()
        {
            InitializeComponent();
        }

        void LoadStaticResource()
        {
            try
            {
                string[] ImgTmp = Directory.GetFiles(Program.PathImgCards);
                Program.ImgCards = new Models.LocalResource.ImageCardsOfDisk[ImgTmp.Length];
                for (int i = 0; i < ImgTmp.Length; i++)
                {
                    Program.ImgCards[i] = new Models.LocalResource.ImageCardsOfDisk() { Img = Image.FromFile(ImgTmp[i]), Name = Path.GetFileNameWithoutExtension(ImgTmp[i]) };
                }
            }
            catch (Exception e)
            {

                //throw;
            }
            //bgw_LoadData.ReportProgress(10, "Идет загрузка статических ресурсов...");

            //bgw_LoadData.ReportProgress(20, "Загрузка статических ресурсов завершена...");
        }


        async void LoadInfo()
        {
            try
            {
                #region Загрузка картинок карт
                //Core core = new Core();
                //Core.Init(mtb_Token.Text, ClashRoyaleAPI.Enums.VersionAPI.v1);
                //Cards CardList = new Cards();
                //string PathImgCards = Path.Combine(Application.StartupPath, "Images", "Cards");
                //if (!Directory.Exists(PathImgCards))
                //{
                //    Directory.CreateDirectory(PathImgCards);
                //}
                //var img = CardList.GetAllCards();
                //string localFilename = @"c:\localpath\tofile.jpg";
                //foreach (var item in img.items)
                //{
                //    using (WebClient client = new WebClient())
                //    {
                //        client.DownloadFile(item.iconUrls.medium,Path.Combine(PathImgCards, $"{item.name}.png"));
                //    }
                //} 
                #endregion


                ReportProgress(20, "Идет загрузка информации о игроке...");
                Program.MyPlayerProfile = await GetPlayerInfoWF(ClashRoyaleClanMonitorWF.Properties.Settings.Default.PlayerID);
                ReportProgress(30, "Идет загрузка информации о предстоящих сундуках...");
                Program.MyChests = await GetChestsInfoWF(Program.MyPlayerProfile.tag);//player.GetUpcomingChestsPlayer(PlayerInfo.tag);
                Program.ClanInfo = await GetClanInfoWF(Program.MyPlayerProfile.clan.tag);
                //ClashRoyaleClanMonitorWF.Controls.Players.PlayerInfo playerInfo = new ClashRoyaleClanMonitorWF.Controls.Players.PlayerInfo(0, MyPlayerProfile, MyChests);
                ReportProgress(40, "Идет загрузка информации о войнах...");
                Program.ClanWarLog = await GetClanWarLogWF(Program.MyPlayerProfile.clan.tag);

                ReportProgress(50, "Идет загрузка информации о клане...");
                Program.ClanMembers = await GetClanMembersInfoWF(Program.MyPlayerProfile.clan.tag); //clan.GetClanMembers(PlayerInfo.clan.tag);
                int value = (100 - Program.ClanMembers.items.Length);
                ReportProgress(value, "Идет загрузка информации о клане...");
                Program.ClanMembersDetailInfo = new Player[Program.ClanMembers.items.Length];
                Program.ClanMembersChests = new UpcomingChests[Program.ClanMembers.items.Length];
                for (int i = 0; i < Program.ClanMembers.items.Length; i++)
                {
                    Program.ClanMembersDetailInfo[i] = await GetPlayerInfoWF(Program.ClanMembers.items[i].tag);
                    Program.ClanMembersChests[i] = await GetChestsInfoWF(Program.ClanMembers.items[i].tag);
                    ReportProgress(value, $"Идет загрузка информации о {Program.ClanMembers.items[i].name}...");
                    value++;
                }
                ReportProgress(value, "Информация загружена...");
                if (mainUC != null)
                {
                    mainUC.Dispose();
                    mainUC = new MainUC();
                    mainUC.Dock = DockStyle.Fill;
                    MainPanel.Controls.Add(mainUC);
                }
                else
                {
                    mainUC = new MainUC();
                    mainUC.Dock = DockStyle.Fill;
                    MainPanel.Controls.Add(mainUC);
                }
                if (clanCoreUC !=null)
                {
                    clanCoreUC.Dispose();
                    clanCoreUC = new ClanCoreUC();
                    clanCoreUC.Dock = DockStyle.Fill;
                }
                else
                {
                    clanCoreUC = new ClanCoreUC();
                    clanCoreUC.Dock = DockStyle.Fill;
                }
                settingsUC.Dock = DockStyle.Fill;

                //GC.Collect();
                //pgbar_Loading.ProgressBarStyle = ProgressBarStyle.Marquee;
                //var T6 = clan.GetClanWarLog(PlayerInfo.clan.tag);
                ////var T7 = clan.GetClanCurrentWar(PlayerInfo.clan.tag);
                //List<Player> tmpWarlogPlayers = new List<Player>();
                //foreach (var item in ListPlayer)
                //{
                //    var ttt = T6.items[1].participants.Where(c => c.name == item.name);
                //    if (ttt == null)
                //    {
                //        tmpWarlogPlayers.Add(item);
                //    }
                //    var ttt2 = T6.items[2].participants.Where(c => c.name == item.name);
                //    if (ttt2 == null)
                //    {
                //        tmpWarlogPlayers.Add(item);
                //    }

                //    //foreach (var item2 in T6.items)
                //    //{
                //    //    foreach (var item3 in item2.participants)
                //    //    {
                //    //        if (item.name == item3.name)
                //    //        {
                //    //            break;
                //    //        }

                //    //    }
                //    //    //item2.participants
                //    //}
                //}
                //var T6 = clan.GetClanWarLog("2LUGU2UY");
                //var T7 = clan.GetClanCurrentWar("2LUGU2UY");
                //});
                //bgw_LoadData.ReportProgress(100, "Вся необходимая информация загружена...");
            }
            catch (ClashRoyaleAPIException e)
            {
                MetroFramework.MetroMessageBox.Show(this, e.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception e)
            {
                MetroFramework.MetroMessageBox.Show(this, e.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void metroTile1_Click(object sender, EventArgs e)
        {
            LoadInfo();
        }

        private void btn_Main_Click(object sender, EventArgs e)
        {
            //this.InvokeEx(() => {
            if (mainUC!=null)
            {
                MainPanel.Controls.Clear();
                MainPanel.Controls.Add(mainUC);
            }

            //});
            //MainPanel.Controls.Add(settingsUC);
        }

        private void btn_Clan_Click(object sender, EventArgs e)
        {
            if (clanCoreUC != null)
            {
                MainPanel.Controls.Clear();
                MainPanel.Controls.Add(clanCoreUC);
            }
            //MainPanel.InvokeEx(new Action(() => {

            //}));
        }

        private void btn_Settings_Click(object sender, EventArgs e)
        {
            if (settingsUC != null)
            {
                MainPanel.Controls.Clear();
                MainPanel.Controls.Add(settingsUC);
            }
            //this.InvokeEx(() => {

            //});
        }

        #region STYLES
        private void btn_Main_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeSidePanelPosition(btn_Main);
        }

        private void btn_Clan_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeSidePanelPosition(btn_Clan);
        }

        private void btn_Settings_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeSidePanelPosition(btn_Settings);
        }

        private void ChangeSidePanelPosition(Control control)
        {
            SidePanel.Height = control.Height;
            SidePanel.Top = control.Top;
        }
        #endregion

        #region System Methods
        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Maximize_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void btn_Minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void mpnl_LeftMenu_MouseMove(object sender, MouseEventArgs e)
        {
            if (downPoint == Point.Empty)
            {
                return;
            }
            Point location = new Point(
                this.Left + e.X - downPoint.X,
                this.Top + e.Y - downPoint.Y);
            this.Location = location;
        }

        private void mpnl_LeftMenu_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            downPoint = new Point(e.X, e.Y);
        }

        private void mpnl_LeftMenu_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            downPoint = Point.Empty;
        }

        public Point downPoint = Point.Empty;

        private void MainPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            downPoint = Point.Empty;
        }

        private void MainPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (downPoint == Point.Empty)
            {
                return;
            }
            Point location = new Point(
                this.Left + e.X - downPoint.X,
                this.Top + e.Y - downPoint.Y);
            this.Location = location;
        }

        private void MainPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            downPoint = new Point(e.X, e.Y);
        }
        #endregion

        private void bgw_LoadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
           var t = e.Result;
        }

        private void bgw_LoadData_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgbar_Loading.Value = e.ProgressPercentage;
        }

        private void bgw_LoadData_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                bgw_LoadData.ReportProgress(0,"Идет загрузка информации...");
                
            }
            catch (Exception)
            {

            }
            e.Result = true;
        }

        private async void FormMain_Load(object sender, EventArgs e)
        {
            ReportProgress(0, "Идет загрузка статических ресурсов");
            LoadStaticResource();
            LoadInfo();
            //bgw_LoadData.RunWorkerAsync();
        }

        public async Task<Player> GetPlayerInfoWF(string ID)
        {
            return await Task.Factory.StartNew(() => { return player.GetPlayerInfo(ID); });
        }

        public async Task<UpcomingChests> GetChestsInfoWF(string ID)
        {
            return await Task.Factory.StartNew(() => { return player.GetUpcomingChestsPlayer(ID); });
        }

        public async Task<ClanMembers> GetClanMembersInfoWF(string ID)
        {
            return await Task.Factory.StartNew(() => {
                return clan.GetClanMembers(ID);            
            });
        }

        public async Task<ClashRoyaleAPI.Models.Clans.Clan> GetClanInfoWF(string ID)
        {
            return await Task.Factory.StartNew(() => {
                return clan.GetClanInfo(ID);
            });

        }

        public async Task<ClashRoyaleAPI.Models.Clans.Warlog> GetClanWarLogWF(string ID)
        {
            return await Task.Factory.StartNew(() => {
                return clan.GetClanWarLog(ID);
            });

        }
        private void ReportProgress(int value, string text)
        {
            pgbar_Loading.ProgressBarStyle = ProgressBarStyle.Continuous;
            pgbar_Loading.Value = value;
            mlbl_ProgressText.Text = text;
        }

    }
}