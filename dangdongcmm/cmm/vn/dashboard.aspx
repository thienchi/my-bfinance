<%@ Page Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="dashboard.aspx.cs" Inherits="dangdongcmm.cmm.dashboard" %>
<%@ REGISTER TagPrefix="UC" TagName="Accesscounter" Src="usercontrol/ucaccesscounter.ascx" %>

<ASP:CONTENT ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
        <CONTENTTEMPLATE>
            <ASP:PANEL ID="pnlLoginfirst" runat="server" Visible="false">
                <div class="FLOUTNOTICESHOW" style="font-weight:normal;">
                    <div class="title">Xin chào, <ASP:LABEL ID="lblUsername" runat="server"></ASP:LABEL></div>
                    <div>
                    Đây là lần đầu tiên bạn đăng nhập hệ thống quản trị website CMM, bạn nên <a href="changepassword.aspx">thay đổi mật khẩu</a> để an toàn hơn.
                    </div>
                </div>
            </ASP:PANEL>
            
            <ASP:PANEL ID="pnlForAdmin" runat="server" Visible="false">
                <div class="firstnotice">
                <br />
                <img src="../images/icon_newmessage.gif" align="absmiddle" /> Thông báo từ hệ thống:
                <br /><ASP:LABEL ID="Count_Feedback" runat="server" Text="<a href='feedbackl.aspx' class='watting'>($COUNT$) thư liên hệ</a><br />"></ASP:LABEL>
                <ASP:LABEL ID="Count_Updating" runat="server" Text="<br /><a href='waittingupdate.aspx' class='watting'>($COUNT$) yêu cầu cập nhật thông tin</a>"></ASP:LABEL>
                <ASP:LABEL ID="Count_Deleting" runat="server" Text="<br /><a href='waittingdelete.aspx' class='watting'>($COUNT$) yêu cầu xóa thông tin</a>"></ASP:LABEL>
                </div>
            </ASP:PANEL>
            <br />&nbsp;
            <UC:ACCESSCOUNTER ID="Accesscounter" runat="server" />
            
        </CONTENTTEMPLATE>
    </ASP:UPDATEPANEL>
</ASP:CONTENT>
