﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" MasterPagefile="Site.Master" Inherits="SACCOPortal.Reports" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <p></p>
    <p></p>
    <% 
     if (Request.QueryString["r"] == "lr")
            {
            %>
  <div class="panel panel-default">
                <div class="panel-heading">Fosa Statement</div>
                <div class="panel-body">

                    <p>Select Loan Number <asp:DropDownList ID="ddFosaAccount" runat="server" CssClass="form-control"></asp:DropDownList></p>
                    <asp:Button ID="btnView" runat="server" Text="View Repayment" CssClass="btn btn-primary btn-lg" OnClick="btnView_Click" />
                </div>    
            </div>
    <%
    }
    %>
    <%--<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:Timer ID="timer1" runat="server" Interval="1000" OnTick="timer1_OnTick"></asp:Timer>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
         <Triggers>
                <asp:AsyncPostBackTrigger controlid="timer1" eventname="Tick" />
           </Triggers>
            <ContentTemplate>
                 <asp:TextBox runat="server" ID="txtTimer" ReadOnly="True"></asp:TextBox>
            </ContentTemplate>
    </asp:UpdatePanel>

    <asp:Button runat="server" ID="btnClickTimer" Text="View Statement" CssClass="btn btn-primary btn-sm" OnClick="btnClickTimer_OnClick"/>--%>
   
   <iframe runat="server" id="pdfReport" width="100%" height="500" src=""></iframe>
 </asp:Content>
