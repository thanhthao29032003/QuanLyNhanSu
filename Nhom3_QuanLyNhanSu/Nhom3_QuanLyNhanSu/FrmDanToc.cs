﻿using System;
using System.Windows.Forms;
using Nhom3_QuanLyNhanSu.Models;
using Nhom3_QuanLyNhanSu.Entities;

namespace Nhom3_QuanLyNhanSu
{
    public partial class FrmDanToc : Form
    {
        #region Function
        private infoTab tab = null;
        public ChangeStateButton change;

        private Validate validate = new Validate();

        private bool BeforeInsert = false;


        public void setTab(infoTab tab)
        {
            this.tab = tab;

            this.tab.AddNew = new infoTab.EventButton(Them);
            this.tab.Cancel = new infoTab.EventButton(HuyBo);
            this.tab.Save = new infoTab.EventButton(LuuLai);
            this.tab.Delete = new infoTab.EventButton(Xoa);
            this.tab.Edit = new infoTab.EventButton(Sua);
            this.tab.Refresh = new infoTab.EventButton(CapNhat);
            this.tab.Print = new infoTab.EventButton(In);
        }

        private void changeStateButton(bool b1, bool b2, bool b3, bool b4, bool b5)
        {
            if (!isUpdate)
            {
                tab.Them = b1;
                tab.Sua = b2;
                tab.Luu = b3;
                tab.CapNhat = b4;
                tab.In = false;

                change();
            }
        }

        private void HiddenIconTextBox()
        {
            lblIconMaCV.Visible = false;
            lblIconTenCV.Visible = false;
            lblIconPhuCap.Visible = false;
        }

        private void EnableTextBox(bool b)
        {
           
            txtTenDanToc.Enabled = b;
            txtPhuCap.Enabled = b;
        }

        public void ShowEditAnDeleteButton()
        {
            if (dataGridView1.Rows.Count > 0)
            {
                if (isAdmin)
                {
                    changeStateButton(true, true, false, true, true);
                }
                else
                {
                    changeStateButton(false, false, false, true, true);
                }
                EnableTextBox(false);
            }
            HiddenIconTextBox();

        }

        private void ShowDetail(DataGridViewRow row)
        {
            if (row != null && (tab != null && (tab.action == ActionForm.KHONG || tab.action==ActionForm.SUA)) || tab == null || BeforeInsert)
            {
                txtMaDanToc.Text = row.Cells[0].Value.ToString();
                txtTenDanToc.Text = row.Cells[1].Value.ToString();
                txtPhuCap.Text = string.Format("{0:0,0}", double.Parse(row.Cells[2].Value.ToString()));
                txtSoNV.Text = row.Cells[3].Value.ToString();
            }
            else {
                clearInsert();
            }
        }

        private void clearInsert(){
            txtMaDanToc.Text = "";
            txtTenDanToc.Text = "";
            txtPhuCap.Text = "";
            txtSoNV.Text = "";
        }

        private void Them()
        {
            changeStateButton(false, false, true, false, true);
            tab.action = ActionForm.THEM;

            EnableTextBox(true);
            clearInsert();
            txtTenDanToc.Focus();
            btnSearch.Enabled = false;
            dataGridView1.Enabled = false;
        }

        private void HuyBoFull()
        {
            changeStateButton(true, true, false, true, true);
            tab.action = ActionForm.KHONG;

            EnableTextBox(false);


            HiddenIconTextBox();
            BeforeInsert = false;
            btnSearch.Enabled = true;
            dataGridView1.Enabled = true;
        }

        private void HuyBo()
        {
            HuyBoFull();
            ShowDetail(dataGridView1.CurrentRow);
        }

        private void LuuLai()
        {
            switch (tab.action)
            {
                case ActionForm.THEM:
                    validate.Check(new ValidateParam(ValidateType.NULL, txtTenDanToc.Text, lblIconTenCV, "Vui lòng nhập tên dân tộc"));
                    validate.Check(new ValidateParam(ValidateType.PRICE, txtPhuCap.Text, lblIconPhuCap, "Phụ cấp phải có dạng tiền tệ."));
                    if (!validate.Check(lblIconMaCV, lblIconTenCV, lblIconPhuCap))
                    {
                        MessageBox.Show("Có lỗi. Không thể lưu");
                        return;
                    }
                    BeforeInsert = true;
                    model.insert(new DanToc() {Ten = validate.formatStringToName(txtTenDanToc.Text), PhuCap = validate.formatPrice(txtPhuCap.Text.Trim()) });
                    HuyBoFull();
                    isChangeData = true;
                    if (!isUpdate && lblMessage.Text.Equals("Insert Successfully"))
                        tab.UpdateData(2);
                    break;
                case ActionForm.SUA:
                    validate.Check(new ValidateParam(ValidateType.NULL, txtTenDanToc.Text, lblIconTenCV, "Vui lòng nhập tên dân tộc"));
                    validate.Check(new ValidateParam(ValidateType.PRICE, txtPhuCap.Text, lblIconPhuCap, "Phụ cấp phải có dạng tiền tệ."));
                    if (!validate.Check(lblIconTenCV,lblIconPhuCap))
                    {
                        MessageBox.Show("Có lỗi. Không thể lưu");
                        return;
                    }
                    model.update(new DanToc() { Ma = txtMaDanToc.Text, Ten = validate.formatStringToName(txtTenDanToc.Text), PhuCap = validate.formatPrice(txtPhuCap.Text.Trim()), SoNV = int.Parse(txtSoNV.Text) });
                    HuyBoFull();
                    isChangeData = true;
                    if (!isUpdate && lblMessage.Text.EndsWith("row(s) affected"))
                        tab.UpdateData(2);
                    break;
            }
        }

        private void Sua()
        {
            changeStateButton(false, false, true, false, true);
            tab.action = ActionForm.SUA;

            EnableTextBox(true);
            txtTenDanToc.Focus();
            btnSearch.Enabled = false;
        }

        private void Xoa()
        {
            if (tab.action == ActionForm.KHONG)
            {
                model.delete();
                if (!isUpdate && lblMessage.Text.EndsWith("row(s) affected"))
                {
                    tab.UpdateData(2);
                }
            }
        }

        private void CapNhat()
        {
            if (tab.action == ActionForm.KHONG)
            {
                model.getAllData();
                ShowDetail(dataGridView1.CurrentRow);
            }
        }

        private void In()
        {
        }

        #endregion

        private DanTocModel model = null;

        private bool isAdmin;

        private bool isUpdate = false;

        public bool isChangeData = false;

        public string CurrentRowSelected = null;

        public FrmDanToc(bool isAdmin)
        {
            InitializeComponent();
            this.isAdmin = isAdmin;
        }

        private void EnableButtonUpdate(bool b) {
            btnLuu.Enabled = !b;
            btnHuy.Enabled = !b;
            btnThem.Enabled = b;
            btnSua.Enabled = b;
        }

        public FrmDanToc(string CurrentRowSelected)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            InitializeComponent();

            this.tab = new infoTab();
           

            this.isUpdate = true;
            this.CurrentRowSelected = CurrentRowSelected;
            EnableTextBox(false);
            HiddenIconTextBox();

            btnLuu.Visible = true;
            btnHuy.Visible = true;
            btnThem.Visible = true;
            btnSua.Visible = true;
            btnDong.Visible = true;

            EnableButtonUpdate(true);

            
        }

        private void FrmDanToc_Load(object sender, EventArgs e)
        {
            model = new DanTocModel();
            model.setControl(dataGridView1, lblMessage);

            model.getAllData();


            model.ShowDetail(ShowDetail);

            validate.SetTooltip(toolTip1);

            if (CurrentRowSelected != null) {
                dataGridView1.ClearSelection();
                foreach (DataGridViewRow row in dataGridView1.Rows) {
                    if (row.Cells[0].Value.ToString().Equals(CurrentRowSelected)) {
                        row.Cells[0].Selected = true;
                    }
                }
                dataGridView1.DoubleClick += new EventHandler(dataGridView1_DoubleClick);
            }

            ShowDetail(dataGridView1.CurrentRow);

            
        }

        private void CloseForm() {
            if (dataGridView1.CurrentRow != null)
                CurrentRowSelected = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            else
                CurrentRowSelected = null;
            this.Dispose();
        }

        void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void txtTenDanToc_Leave(object sender, EventArgs e)
        {
            validate.Check(new ValidateParam(ValidateType.NULL, txtTenDanToc.Text, lblIconTenCV, "Vui lòng nhập tên dân tộc"));
        }

        private void txtPhuCap_Leave(object sender, EventArgs e)
        {
            validate.Check(new ValidateParam(ValidateType.PRICE, txtPhuCap.Text, lblIconPhuCap, "Phụ cấp phải có dạng tiền tệ."));
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            model.getAllData("TEN_CV", txtKey.Text.Trim());
            button1.Enabled = true;
            ShowDetail(dataGridView1.CurrentRow);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            model.getAllData();
            button1.Enabled = false;
            ShowDetail(dataGridView1.CurrentRow);
        }

        private void txtKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSearch.PerformClick();
            }
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            Them();
            EnableButtonUpdate(false);
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            LuuLai();
            if (tab.action == ActionForm.KHONG)
            EnableButtonUpdate(true);
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            HuyBo();
            EnableButtonUpdate(true);
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            Sua();
            EnableButtonUpdate(false);
        }


    }
}
