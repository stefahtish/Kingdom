﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Providers.Entities;
using System.Web.UI;
using System.Web.UI.WebControls;
using SACCOPortal.NavOData;
using System.Drawing;

namespace SACCOPortal
{
    public partial class StandingOrdersJoint : System.Web.UI.Page
    {
        public NAV nav = new NAV(new Uri(ConfigurationManager.AppSettings["ODATA_URI"]))
        {
            Credentials =
            new NetworkCredential(ConfigurationManager.AppSettings["W_USER"], ConfigurationManager.AppSettings["W_PWD"],
                ConfigurationManager.AppSettings["DOMAIN"])
        };
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usernameJ"] == null)
            {
                Response.Redirect("Default.aspx");
            }
            Session["usernameJointBosa"] = WSConfig.ObjNav.FnJointusername(Session["usernameJ"].ToString());
            if (!IsPostBack)
            {
                LoadStandingOrders(nav);

                //loadDestAccs();
                if (!string.IsNullOrEmpty(lblSTONo.Text))
                {
                    EditSto();
                }
                else
                {
                    LoadAccounts(nav);
                    LoadApprovedStos(nav);
                    btnupdtstndOd.Visible = false;
                    //btnupdtstndOd.Visible = true;
                }

            }


        }
        protected void LoadStandingOrders(NAV navData)
        {
            var navObj = navData.StandingOrders.Where(r => r.BOSA_Account_No == Session["usernameJointBosa"].ToString() && r.Status == "Open").ToList();

            grdViewStandingOrders.DataSource = navObj;
            grdViewStandingOrders.AutoGenerateColumns = false;
            grdViewStandingOrders.AutoGenerateDeleteButton = false;
            grdViewStandingOrders.AutoGenerateEditButton = false;
            grdViewStandingOrders.DataBind();

        }
        protected void LoadAccounts(NAV navData)
        {
            try
            {
                var sacc = new List<string>();

                string fosadesc = WSConfig.ObjNav.FnMemberAccunts(Session["usernameJointBosa"].ToString());

                var sdetails = fosadesc.Split(new string[] { "::::" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < sdetails.Length; i++)
                {
                    sacc.Add(sdetails[i]);
                }

                ddlSourceAcc.DataSource = sacc;
                ddlSourceAcc.DataBind();

                ddlSourceAcc.Items.Insert(0, "--select account--");
            }
            catch (Exception exception)
            {
                exception.Data.Clear();
            }
        }

        protected void LoadApprovedStos(NAV navData)
        {
            var navObj = navData.StandingOrders.Where(r => r.BOSA_Account_No == Session["usernameJointBosa"].ToString() && r.Status == "Approved").ToList();

            gridapproavedSTOs.DataSource = navObj;
            gridapproavedSTOs.AutoGenerateColumns = false;
            gridapproavedSTOs.AutoGenerateDeleteButton = false;
            gridapproavedSTOs.AutoGenerateEditButton = false;
            gridapproavedSTOs.DataBind();
        }



        protected void LoadDestAccs()
        {
            try
            {
                ddlDestAccName.Items.Clear();

                string[] sourceAcc = ddlSourceAcc.SelectedValue.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                string accNo = sourceAcc[0];

                var dacc = new List<string>();
                string[] ddetails;

                string fosadesc = WSConfig.ObjNav.FnMemberAccuntsdest(Session["usernameJointBosa"].ToString(), accNo);

                ddetails = fosadesc.Split(new string[] { "::::" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < ddetails.Length; i++)
                {
                    dacc.Add(ddetails[i]);
                }

                ddlDestAccName.DataSource = dacc;
                ddlDestAccName.DataBind();

                ddlDestAccName.Items.Insert(0, "--select account--");

                //SACCOFactory.ShowAlert(dacc.ToString());

            }
            catch (Exception exception)
            {
                exception.Data.Clear();
            }

            //ddlDestAccName.Items.Clear();
            //var objNav = nav.FosaAccounts.Where(r => r.BOSA_Account_No == Session["username"].ToString()).ToList();

            //ddlDestAccName.DataSource = objNav;
            //ddlDestAccName.DataTextField = "Account_Type";
            //ddlDestAccName.DataValueField = "No";
            //ddlDestAccName.DataBind();
            //ddlDestAccName.Items.Insert(0, "--select account--");         
        }

        protected void SaveStandingOrdersSavings()
        {
            try
            {
                DateTime sttdate;
                DateTime endate;
                string bsaNo = Session["usernameJointBosa"].ToString();

                string[] sdetails = ddlSourceAcc.SelectedValue.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                string accNo = sdetails[0];

                string freq = ddlFrequency.SelectedValue;
                string desttype = ddlDestAccTy.SelectedValue;


                string[] ddetails = ddlDestAccName.SelectedValue.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                string destacNo = ddetails[0];
                string stodescrptn = txtstoDescrp.Text.Trim();

                string loanno = ddlLoanNo.SelectedValue;

                string amount = txtAmount.Text;
                decimal amt = Convert.ToDecimal(amount);

                var stdt = txtstrtDate.Value;
                if (string.IsNullOrWhiteSpace(stdt))
                {
                    SACCOFactory.ShowAlert("Please Select date");
                    txtstrtDate.Focus();
                    return;
                }
                else
                {
                    sttdate = DateTime.Parse(stdt);
                }

                var endt = txtendate.Value;
                if (string.IsNullOrWhiteSpace(endt))
                {
                    SACCOFactory.ShowAlert("Please select end date");
                    txtendate.Focus();
                    return;
                }
                else
                {
                    endate = DateTime.Parse(endt);
                }


                DateTime nowTime = DateTime.Now;

                if (ddlFrequency.SelectedIndex == 0)
                {
                    SACCOFactory.ShowAlert("Please select a valid frequency");
                }

                if (string.IsNullOrEmpty(amount))
                {
                    SACCOFactory.ShowAlert("Please enter STO Amount!");
                }

                int startDateError = DateTime.Compare(sttdate, nowTime);
                if (startDateError <= 0)
                {
                    SACCOFactory.ShowAlert("Adjust Start Date be today or later");
                    return;
                }
                int endDateError = DateTime.Compare(endate, nowTime);
                if (endDateError < 0)
                {
                    SACCOFactory.ShowAlert("Adjust End Date to a later date");
                    return;
                }
                //var accType = ddlDestAccTy.SelectedValue;

                //switch (accType)
                //{
                //    case "Inter Savings":
                //        WSConfig.ObjNav.FnStandingOrders(bsaNo, accNo, freq, destacNo, sttdate, amt, endate, stodescrptn);
                //        break;

                //    case "BOSA":
                //        WSConfig.ObjNav.FnStandingOrdersBosa(bsaNo, accNo, freq, sttdate, amt, endate, stodescrptn, loanno);
                //        break;
                //}

                WSConfig.ObjNav.FnStandingOrders(bsaNo, accNo, freq, destacNo, sttdate, amt, endate, stodescrptn);
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "myFunction();", true);
                    //SACCOFactory.ShowAlert("Standing Order Created successfully!");
                    LoadStandingOrders(nav);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "text", "ShownDestaccDIV()", true);
                    //Response.Redirect("StandingOrders.aspx");
                }

            }
            catch (Exception ex)
            {
                SACCOFactory.ShowAlert(ex.Message);
            }
        }

        protected void SaveStandingordersBosa()
        {
            try
            {
                DateTime sttdate;
                DateTime endate;

                string bsaNo = Session["usernameJointBosa"].ToString();

                string[] sdetails = ddlSourceAcc.SelectedValue.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                string accNo = sdetails[0];

                string freq = ddlFrequency.SelectedValue;
                string desttype = ddlDestAccTy.SelectedValue;


                string[] ddetails = ddlDestAccName.SelectedValue.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                string destacNo = ddetails[0];
                string stodescrptn = txtstoDescrp.Text.Trim();

                string loanno = ddlLoanNo.SelectedValue;

                string amount = txtAmount.Text;
                decimal amt = Convert.ToDecimal(amount);

                var stdt = txtstrtDate.Value;
                if (string.IsNullOrWhiteSpace(stdt))
                {
                    SACCOFactory.ShowAlert("Please Select date");
                    txtstrtDate.Focus();
                    return;
                }
                else
                {
                    sttdate = DateTime.Parse(stdt);
                }

                var endt = txtendate.Value;
                if (string.IsNullOrWhiteSpace(endt))
                {
                    SACCOFactory.ShowAlert("Please select end date");
                    txtendate.Focus();
                    return;
                }
                else
                {
                    endate = DateTime.Parse(endt);
                }


                DateTime nowTime = DateTime.Now;

                if (ddlFrequency.SelectedIndex == 0)
                {
                    SACCOFactory.ShowAlert("Please select a valid frequency");
                }

                if (string.IsNullOrEmpty(amount))
                {
                    SACCOFactory.ShowAlert("Please enter STO Amount!");
                }

                int startDateError = DateTime.Compare(sttdate, nowTime);
                if (startDateError <= 0)
                {
                    SACCOFactory.ShowAlert("Adjust Start Date be today or later");
                    return;
                }
                int endDateError = DateTime.Compare(endate, nowTime);
                if (endDateError < 0)
                {
                    SACCOFactory.ShowAlert("Adjust End Date to a later date");
                    return;
                }

                //WSConfig.ObjNav.FnStandingOrders(bsaNo, accNo, freq, destacNo, sttdate, amt, endate, stodescrptn);

                string docno = WSConfig.ObjNav.FnStandingOrdersBosa(bsaNo, accNo, freq, sttdate, amt, endate, stodescrptn);
                WSConfig.ObjNav.FnreceipteallocationBosa(loanno, bsaNo, docno, amt);
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "myFunction();", true);
                    //SACCOFactory.ShowAlert("Standing Order Created successfully!");
                    LoadStandingOrders(nav);
                    dvBosa.Visible = true;
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "text", "ShownBosaDIV()", true);
                    //Response.Redirect("StandingOrders.aspx");
                }

            }
            catch (Exception ex)
            {
                SACCOFactory.ShowAlert(ex.Message);
            }

        }

        protected void stadingOrderbtn_Click(object sender, EventArgs e)
        {
            SaveStandingOrdersSavings();
        }

        protected void grdViewStandingOrders_RowEditing(object sender, GridViewEditEventArgs e)
        {
            EditStandingorders();
        }
        protected void EditStandingorders()
        {
            SACCOFactory.ShowAlert("editing");
        }

        protected void grdViewStandingOrders_OnSelectedIndexChanged(object sender, EventArgs e)
        {

            Session["sto_no"] = grdViewStandingOrders.SelectedRow.Cells[1].Text;
            EditSto();
        }

        protected void EditSto()
        {
            var stono = Session["sto_no"].ToString();
            lblSTONo.Text = stono;
            stadingOrderbtn.Visible = false;
            btnupdtstndOd.Visible = true;
            ddlSourceAcc.Enabled = false;


            string destacctype = grdViewStandingOrders.SelectedRow.Cells[3].Text;
            switch (destacctype)
            {
                case "Inter Savings":
                    ddlDestAccTy.SelectedItem.Text = "Inter Savings";
                    ddlDestAccTy.Enabled = false;
                    //ddlFrequency.Enabled = false;
                    txtstoDescrp.Enabled = false;
                    destaccIntersavings.Visible = true;
                    dvBosa.Visible = false;
                    dvOtherAccs.Visible = false;
                    //txtDestAccOther.Enabled = false;
                    txtdest.Enabled = false;
                    txtdest.Visible = true;

                    string destacc = grdViewStandingOrders.SelectedRow.Cells[4].Text;
                    string fosacdest = WSConfig.ObjNav.FnMemberAccuntsSTOEDIT(destacc);
                    //SACCOFactory.ShowAlert(fosacdest.ToString());
                    txtdest.Text = fosacdest;
                    break;
                case "BOSA":
                    ddlDestAccTy.SelectedItem.Text = "BOSA";
                    ddlDestAccTy.Enabled = false;
                    //ddlFrequency.Enabled = false;
                    txtstoDescrp.Enabled = false;
                    dvBosa.Visible = true;
                    destaccIntersavings.Visible = false;
                    dvOtherAccs.Visible = false;
                    ddlLoanNo.Enabled = false;
                    break;
                case "Other Banks":
                    ddlDestAccTy.SelectedItem.Text = "Other Account";
                    dvOtherAccs.Visible = true;
                    ddlDestAccTy.Enabled = false;
                    txtstoDescrp.Enabled = false;
                    //ddlFrequency.Enabled = false;
                    destaccIntersavings.Visible = false;
                    dvBosa.Visible = false;
                    txtDestAccOther.Enabled = false;
                    string destac = grdViewStandingOrders.SelectedRow.Cells[4].Text;
                    string otheracc = WSConfig.ObjNav.FnGetAccountName(destac);
                    txtDestAccOther.Text = otheracc;
                    break;
            }

            string freq = grdViewStandingOrders.SelectedRow.Cells[8].Text;
            switch (freq)
            {
                case "1D":
                    ddlFrequency.SelectedItem.Text = "Daily";
                    break;
                case "1W":
                    ddlFrequency.SelectedItem.Text = "Weekly";
                    break;
                case "1M":
                    ddlFrequency.SelectedItem.Text = "Monthly";
                    break;
            }

            txtAmount.Text = grdViewStandingOrders.SelectedRow.Cells[9].Text;
            txtstrtDate.Value = grdViewStandingOrders.SelectedRow.Cells[5].Text;
            txtendate.Value = grdViewStandingOrders.SelectedRow.Cells[6].Text;
            txtstoDescrp.Text = grdViewStandingOrders.SelectedRow.Cells[7].Text;


            string srcAc = grdViewStandingOrders.SelectedRow.Cells[2].Text;
            string fosadesc = WSConfig.ObjNav.FnMemberAccuntsSTOEDIT(srcAc);
            ddlSourceAcc.SelectedItem.Text = fosadesc.ToString();
        }

        protected void DeleteSto(string stndno)
        {
            if (WSConfig.ObjNav.FnCancelStandingOrders(stndno) == true)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "deleteSto();", true);
                //SACCOFactory.ShowAlert("Standing Order Cancelled successfully");
            }
        }

        protected void btnupdtstndOd_OnClick(object sender, EventArgs e)
        {
            try
            {
                var stono = Session["sto_no"].ToString();
                lblSTONo.Text = stono;

                string edfreq = ddlFrequency.SelectedValue;
                if (ddlFrequency.SelectedIndex == 0)
                {
                    SACCOFactory.ShowAlert("Please reselect a frequency");
                    ddlFrequency.Focus();
                    return;
                }


                //string edduration = ddlDuration.SelectedValue;
                decimal edamt;
                string edamount = txtAmount.Text;
                if (string.IsNullOrEmpty(edamount))
                {
                    SACCOFactory.ShowAlert("Please enter STO Amount!");
                    txtAmount.Focus();
                    return;
                }
                else
                {
                    edamt = Convert.ToDecimal(edamount);
                }



                DateTime edstartDate;
                var startd = txtstrtDate.Value;
                if (string.IsNullOrWhiteSpace(startd))
                {
                    SACCOFactory.ShowAlert("Please select end date");
                    txtstrtDate.Focus();
                    return;
                }
                else
                {
                    edstartDate = DateTime.Parse(startd);
                }

                DateTime nowTime = DateTime.Now;
                int startDateError = DateTime.Compare(edstartDate, nowTime);
                if (startDateError <= 0)
                {
                    SACCOFactory.ShowAlert("Start Date should be today or later date");
                }

                DateTime edendate;
                var enddt = txtendate.Value;
                if (string.IsNullOrWhiteSpace(enddt))
                {
                    SACCOFactory.ShowAlert("Please select end date");
                    txtendate.Focus();
                    return;
                }
                else
                {
                    edendate = DateTime.Parse(enddt);
                }

                WSConfig.ObjNav.FnEditStandingOrder(stono, edfreq, edstartDate, edamt, edendate);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "updateSto();", true);
                //WSConfig.ObjNav.FnEditStandingOrder(STONO, edfreq, edduration, edstartDate, edamt);
                // SACCOFactory.ShowAlert("Standing Order Edited successfully!");
                LoadStandingOrders(nav);
                //Response.Redirect("StandingOrders.aspx");
            }
            catch (Exception ex)
            {
                //SACCOFactory.ShowAlert("There was an error in creating this standing order");
                SACCOFactory.ShowAlert(ex.Message);
            }
        }

        protected void ddlDestAccName_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ScriptManager.RegisterStartupScript(this, this.GetType(), "text", "ShownDestaccDIV()", true);
            //var noM = ddlDestAccName.SelectedItem.Text;

            //var myAcNm = nav.accountTypes.ToList().Where(r => r.Code == noM).Select(w => w.Description).SingleOrDefault();
            //txtAccName.Text = myAcNm;
        }

        protected void ddlSourceAcc_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                LoadDestAccs();
            }
            catch (Exception exception)
            {
                exception.Data.Clear();
            }
        }

        protected void ddlDestAccTy_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ddlDestAccTy.Items.Clear();
            var myBosaAcc = ddlDestAccTy.SelectedItem.Text;

            switch (myBosaAcc)
            {
                case "Inter Savings":
                    stadingOrderbtn.Visible = true;
                    btnstandingOrderBosa.Visible = false;
                    btnstandingOrderOther.Visible = false;
                    destaccIntersavings.Visible = true;
                    dvBosa.Visible = false;
                    dvOtherAccs.Visible = false;
                    //LoadDestAccs();
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "text", "ShownDestaccDIV()", true);
                    // ScriptManager.RegisterStartupScript(this, this.GetType(), "text", "HideBosaDIV()", true);
                    // ScriptManager.RegisterStartupScript(this, this.GetType(), "text", "HideDestaccOtherDIV()", true);
                    break;

                case "BOSA":
                    btnstandingOrderBosa.Visible = true;
                    btnstandingOrderOther.Visible = false;
                    stadingOrderbtn.Visible = false;
                    dvBosa.Visible = true;
                    dvOtherAccs.Visible = false;
                    destaccIntersavings.Visible = false;
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "text", "ShownBosaDIV()", true);
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "text", "HideDestaccDIV()", true);
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "text", "HideDestaccOtherDIV()", true);
                    LoadBosaLoans();
                    break;

                case "Other Account":
                    btnstandingOrderOther.Visible = true;
                    stadingOrderbtn.Visible = false;
                    btnstandingOrderBosa.Visible = false;
                    dvBosa.Visible = false;
                    dvOtherAccs.Visible = true;
                    destaccIntersavings.Visible = false;
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "text", "ShownDestaccOtherDIV()", true);
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "text", "HideDestaccDIV()", true);
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "text", "HideBosaDIV()", true);
                    break;

                default:
                    stadingOrderbtn.Visible = true;
                    dvBosa.Visible = false;
                    dvOtherAccs.Visible = false;
                    destaccIntersavings.Visible = false;
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "text", "HideBosaDIV()", true);
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "text", "HideDestaccDIV()", true);
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "text", "HideDestaccOtherDIV()", true);
                    break;
            }

        }

        protected void LoadBosaLoans()
        {
            //ddlDestAccTy.Items.Clear();

            var objBosaLoan = nav.LoansR.Where(n => n.Client_Code == Session["usernameJointBosa"].ToString() && n.Source == "BOSA" && n.Loan_Status == "Issued" && n.Posted == true)
                .Select(mc => new { combined = mc.Loan_No + " - " + mc.Loan_Product_Type_Name, LoanNum = mc.Loan_No }).ToList();

            ddlLoanNo.DataSource = objBosaLoan;
            ddlLoanNo.DataTextField = "combined";
            ddlLoanNo.DataValueField = "LoanNum";
            ddlLoanNo.DataBind();
            ddlLoanNo.Items.Insert(0, "..select loan no..");
        }

        protected void ddlLoanNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "text", "ShownBosaDIV()", true);
            dvBosa.Visible = true;
        }

        protected void btnstandingOrderBosa_Click(object sender, EventArgs e)
        {
            SaveStandingordersBosa();
        }

        protected void btnstandingOrderOther_Click(object sender, EventArgs e)
        {

            try
            {
                DateTime sttdate;
                DateTime endate;
                string bsaNo = Session["usernameJointBosa"].ToString();

                string[] sdetails = ddlSourceAcc.SelectedValue.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                string accNo = sdetails[0];

                string freq = ddlFrequency.SelectedValue;
                string desttype = ddlDestAccTy.SelectedValue;





                string destacNo = txtDestAccOther.Text;
                string stodescrptn = txtstoDescrp.Text.Trim();


                string amount = txtAmount.Text;
                decimal amt = Convert.ToDecimal(amount);

                var stdt = txtstrtDate.Value;
                if (string.IsNullOrWhiteSpace(stdt))
                {
                    SACCOFactory.ShowAlert("Please Select date");
                    txtstrtDate.Focus();
                    return;
                }
                else
                {
                    sttdate = DateTime.Parse(stdt);
                }

                var endt = txtendate.Value;
                if (string.IsNullOrWhiteSpace(endt))
                {
                    SACCOFactory.ShowAlert("Please select end date");
                    txtendate.Focus();
                    return;
                }
                else
                {
                    endate = DateTime.Parse(endt);
                }


                DateTime nowTime = DateTime.Now;

                if (ddlFrequency.SelectedIndex == 0)
                {
                    SACCOFactory.ShowAlert("Please select a valid frequency");
                }

                if (string.IsNullOrEmpty(amount))
                {
                    SACCOFactory.ShowAlert("Please enter STO Amount!");
                }

                int startDateError = DateTime.Compare(sttdate, nowTime);
                if (startDateError <= 0)
                {
                    SACCOFactory.ShowAlert("Adjust Start Date be today or later");
                    return;
                }
                int endDateError = DateTime.Compare(endate, nowTime);
                if (endDateError < 0)
                {
                    SACCOFactory.ShowAlert("Adjust End Date to a later date");
                    return;
                }
                if (!WSConfig.ObjNav.FnAccountNo(txtDestAccOther.Text.Trim()))
                {
                    SACCOFactory.ShowAlert("The Destination Account Number specified is inavalid. Kindly Enter the Correct Account Number.");
                    txtDestAccOther.Text = "";
                    txtDestAccOther.Focus();
                    return;
                }
                else
                {
                    string response = WSConfig.ObjNav.FnStandingOrdersOther(bsaNo, accNo, freq, destacNo, sttdate, amt, endate, stodescrptn);
                    {
                        Response.Write("<script language='javascript'>alert('" + response + "'); location.href = 'StandingOrders.aspx';</script>");
                        //ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "myFunction();", true);
                        //SACCOFactory.ShowAlert("Standing Order Created successfully!");
                        LoadStandingOrders(nav);
                        dvOtherAccs.Visible = true;
                    }
                }

                //WSConfig.ObjNav.FnStandingOrdersOther(bsaNo, accNo, freq, destacNo, sttdate, amt, endate, stodescrptn);
                //{
                //        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "myFunction();", true);
                //        //SACCOFactory.ShowAlert("Standing Order Created successfully!");
                //    LoadStandingOrders(nav);
                //    dvOtherAccs.Visible = true;
                //    //ScriptManager.RegisterStartupScript(this, this.GetType(), "text", "ShownDestaccOtherDIV()", true);
                //    //Response.Redirect("StandingOrders.aspx");                    
                //}

            }
            catch (Exception ex)
            {
                SACCOFactory.ShowAlert(ex.Message);
                //ex.Data.Clear();
                //SACCOFactory.ShowAlert("The Destination Account Number does not exist");
                //lblError.Text = "The Destination Account Number does not exist";
                //dvOtherAccs.Visible = true;
                //txtDestAccOther.Focus();
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "text", "ShownDestaccOtherDIV()", true);
                return;

            }
        }

        protected void lnkCancel_OnClick(object sender, EventArgs e)
        {
            var stdn = (sender as LinkButton).CommandArgument;

            DeleteSto(stdn);
        }
    }
}