<%@ Page Language="C#" MasterPageFile="MasterHome.Master" AutoEventWireup="true" CodeBehind="M_profile.aspx.cs" Inherits="dangdongcmm.M_profile" EnableViewState="false" %>

<ASP:CONTENT ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="title2bound">Thông tin tài khoản cá nhân</div>
    
    <div class="tformin">
        <div class="rowhor">
            <div class="title">E-mail đăng nhập:</div> 
            <div class="boxctrlm titlel"><%=CurrentMember.Username %></div>
        </div>
        <div class="rowhor">
            <div class="title">Mật khẩu:</div> 
            <div class="boxctrls note">[ Ẩn ]</div> <div class="boxctrls note"><a href="M_password.aspx">Đổi mật khẩu</a></div>
        </div>
        <div class="rowhor">
            <div class="title">Họ tên:</div> 
            <div class="boxctrlm titlel"><%=CurrentMember.Fullname %></div>
        </div>
        <div class="rowhor">
            <div class="title">Địa chỉ:</div> 
            <div class="boxctrlm titlel"><%=CurrentMember.iProfile.Address%></div>
        </div>
        <div class="rowhor">
            <div class="title">Điện thoại:</div> 
            <div class="boxctrlm titlel"><%=CurrentMember.iProfile.Phone%></div>
        </div>
        <div class="rowhor">
            <div class="title">Yahoo <img src="../images/yahoo-16x16.png" alt="" align="middle" /></div> 
            <div class="boxctrlm titlel"><%=CurrentMember.iProfile.Yahootext%></div>
        </div>
        <div class="rowhor">
            <div class="title">Skype <img src="../images/skype-16x16.png" alt="" align="middle" /></div> 
            <div class="boxctrlm titlel"><%=CurrentMember.iProfile.Skypetext%></div>
        </div>
        <div class="rowhor">
            <div class="title"></div>
        </div>    
        <div class="rowhor">
            <div class="title"></div>
            <div class="boxctrll">
                <input type="button" class="buttonwbgs" value="Chỉnh sửa" onclick="gotoPage('M_profileedit.aspx');" />
            </div>
        </div>
    </div>
    
</ASP:CONTENT>
