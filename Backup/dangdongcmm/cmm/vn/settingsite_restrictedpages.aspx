<%@ Page Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="settingsite_restrictedpages.aspx.cs" Inherits="dangdongcmm.cmm.settingsite_restrictedpages" %>
<%@ MasterType virtualpath="MasterDefault.master" %>

<ASP:CONTENT ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
        <CONTENTTEMPLATE>
        <ASP:LABEL ID="lblError" runat="server" CssClass="dformnoticeb"></ASP:LABEL>
        <ul class="tabs">
	        <li class="first"><a href="#">Các trang cần phải đăng nhập để xem</a></li>
	        <li><a href="#">Các thiết lập cho thành viên</a></li>
        </ul>
        <div class="panes">
            <div class="tformin tformincom">
                <div class="rowhor">
                    <div class="boxctrll note">Thêm vào/ Xóa các trang cần phải đăng nhập để xem</div>
                </div>    
                <div class="rowhor">
                    <div class="title"><span class="require">*</span> Đường dẫn:</div>
                    <div class="boxctrlm"><ASP:TEXTBOX ID="txtName" runat="server"></ASP:TEXTBOX></div>
                    <div class="boxctrls">&nbsp; &nbsp; <ASP:BUTTON ID="cmdAdd" runat="server" CssClass="buttoncmd" Text="Thêm" Width="63px" OnClick="cmdAdd_Click" /></div>
                </div>
                <div class="rowhor">
                    <div class="title">Các trang yêu cầu đăng nhập</div>
                    <div class="boxctrlm"><ASP:LISTBOX ID="ddlPathandQuery" runat="server" Height="150px" SelectionMode="Multiple"></ASP:LISTBOX></div>
                    <div class="boxctrls">&nbsp; &nbsp; <ASP:BUTTON ID="cmdRemove" runat="server" CssClass="buttoncmd" Text="Xóa" OnClick="cmdRemove_Click" /></div>
                </div>
            </div>
            
            <div class="tformin tformincom">
                <div class="rowhor">
                    <div class="title"></div>
                    <div class="boxctrll">
                        <div><b>Sau khi đăng ký, thành viên có cần phải kích hoạt tài khoản hay không</b></div>
                        <ASP:RADIOBUTTONLIST ID="radRegisterconfirm" runat="server" CssClass="ctrlradwi" RepeatDirection="Horizontal">
                            <ASP:LISTITEM Value="0">Không cần</ASP:LISTITEM>
                            <ASP:LISTITEM Value="1">Yêu cầu kích hoạt tài khoản</ASP:LISTITEM>
                        </ASP:RADIOBUTTONLIST>
                    </div>
                </div>    
                <div class="rowhor">
                    <div class="title"></div>
                    <div class="boxctrll">&nbsp;</div>
                </div>    
                <div class="rowhor">
                    <div class="title"></div>
                    <div class="boxctrlm">
                        <div><b>Sau khi đăng nhập sẽ dẫn tới trang</b></div>
                        <ASP:RADIOBUTTONLIST ID="Re_radPage" runat="server" CssClass="ctrlradwi" RepeatDirection="Horizontal" onchange="javascript:Re_radChange();">
                            <ASP:LISTITEM Value="0">Trang truy cập trước đó</ASP:LISTITEM>
                            <ASP:LISTITEM Value="1">Trang khác</ASP:LISTITEM>
                        </ASP:RADIOBUTTONLIST></td>
                    </div>
                    
                </div>    
                <div class="rowhor">
                    <div class="title"></div>
                    <div class="boxctrls" style="margin-right:80px;">&nbsp;</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="Re_txtPage" runat="server"></ASP:TEXTBOX></div>
                </div>    
            </div>
        </div>
        
        <div class="tformin tformincom">
            <br />&nbsp;
            <div class="rowhor">
                <div class="title"></div>
                <div class="boxctrll">
                    <fieldset>
                        <legend>
                            <ASP:BUTTON ID="cmdSave" runat="server" CssClass="button" Text="Lưu thiết lập" OnClientClick="javascript:return doSavedata();" OnClick="cmdSave_Click" />
                        </legend>
                    </fieldset>
                </div>
            </div>
        </div>
        
        </CONTENTTEMPLATE>
    </ASP:UPDATEPANEL>
    
    <script type="text/javascript">
    $(function() { initabs(); });
    function doSave() { return true; }
    
    function Re_radChange() {
        var Re = $('#<%=Re_radPage.ClientID %>').find('input[type=radio][name$="Re_radPage"]:checked').val();
        if(Re == 0) $('#<%=Re_txtPage.ClientID %>').hide();
        else if(Re == 1) $('#<%=Re_txtPage.ClientID %>').show();
    }
    </script>
    
</ASP:CONTENT>
