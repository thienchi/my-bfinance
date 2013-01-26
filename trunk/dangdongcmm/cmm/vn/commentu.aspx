<%@ Page Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="commentu.aspx.cs" Inherits="dangdongcmm.cmm.commentu" %>
<%@ MasterType virtualpath="MasterDefault.master" %>
<%@ REGISTER TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>
<%@ REGISTER TagPrefix="UC" TagName="Displaysetting" Src="usercontrol/ucdisplaysetting.ascx" %>

<ASP:CONTENT ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
        <CONTENTTEMPLATE>
        <ASP:LABEL ID="lblError" runat="server" CssClass="dformnoticeb"></ASP:LABEL>
        <ul class="tabs">
	        <li class="first"><a href="#">Thông Tin Chung</a></li>
	        <li><a href="#">Nội Dung</a></li>
	        <li><a href="#">Thiết Lập Hiển Thị</a></li>
        </ul>
        <div class="panes">
            <div class="tformin tformincom">
                <div class="rowhor">
                    <div class="title"><span class="require">*</span> Tên/ Tiêu đề:</div>
                    <div class="boxctrll"><ASP:TEXTBOX ID="txtName" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title"><span class="require">*</span> Thông tin phản hồi:</div>
                    <div class="boxctrls"><ASP:DROPDOWNLIST ID="ddlCid" runat="server" AutoPostBack="true" onchange="javascript:doChange();" OnSelectedIndexChanged="ddlCid_SelectedIndexChanged"></ASP:DROPDOWNLIST></div>
                    <div class="boxctrlc" style="margin-left:5px;"><ASP:DROPDOWNLIST ID="ddlName" runat="server"></ASP:DROPDOWNLIST></div>
                </div>
                <div class="rowhor">
                    <div class="title">Người gửi:</div>
                    <div class="boxctrlm"><ASP:TEXTBOX ID="txtSender_Name" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">E-mail:</div>
                    <div class="boxctrlm"><ASP:TEXTBOX ID="txtSender_Email" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">Điện thoại:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="txtSender_Phone" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">Địa chỉ:</div>
                    <div class="boxctrlm"><ASP:TEXTBOX ID="txtSender_Address" runat="server"></ASP:TEXTBOX></div>
                </div>
            </div>
            
            <div class="tformin tformincom">
                <div class="rowhor">
                    <div class="title">Giới thiệu ngắn:</div>
                    <div class="boxctrll"><FTB:FREETEXTBOX id="txtIntroduce" runat="Server" width="100%"
								    buttonset="Office2003" toolbarlayout="FontFacesMenu, FontSizesMenu, FontForeColorsMenu , FontForeColorPicker | Bold, Italic, Underline, Strikethrough; Superscript, Subscript, RemoveFormat | JustifyLeft, JustifyRight, JustifyCenter, JustifyFull, BulletedList; NumberedList, Indent, Outdent; CreateLink, Unlink, InsertImage, InsertRule, InsertDiv | Cut, Copy, Paste; Undo, Redo, Print"
								    height="150px"></FTB:FREETEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">Nội dung:</div>
                    <div class="boxctrll"><FTB:FREETEXTBOX id="txtDescription" runat="Server" width="100%"
								    buttonset="Office2003" toolbarlayout="FontFacesMenu, FontSizesMenu, FontForeColorsMenu , FontForeColorPicker | Bold, Italic, Underline, Strikethrough; Superscript, Subscript, RemoveFormat | JustifyLeft, JustifyRight, JustifyCenter, JustifyFull, BulletedList; NumberedList, Indent, Outdent; CreateLink, Unlink, InsertImage, InsertRule, InsertDiv | Cut, Copy, Paste; Undo, Redo, Print"
								    height="400px"></FTB:FREETEXTBOX></div>
                </div>
            </div>
            
            <div class="tformin tformincom">
                <UC:DISPLAYSETTING ID="Displaysetting" runat="server" Showmarkas="true" />
            </div>
        </div>
                
        <div class="tformin tformincom">
            <br />&nbsp;
            <div class="rowhor">
                <div class="title"></div>
                <div class="boxctrll">
                    <ASP:HIDDENFIELD ID="txtId" runat="server" Value="0" />
                    <fieldset>
                        <legend>
                            <ASP:BUTTON ID="cmdSave" runat="server" CssClass="button" Text=" Lưu " OnClientClick="javascript:return doSavedata();" OnClick="cmdSave_Click" />
                            <ASP:BUTTON ID="cmdClear" runat="server" CssClass="button" Text="Nhập lại" OnClick="cmdClear_Click" />
                        </legend>
                        <div>
                            <ASP:CHECKBOX ID="chkSaveoption_golist" runat="server" Text="Lưu thông tin & tiếp tục chỉnh sửa?" />
                            <br />
                            <ASP:CHECKBOX ID="chkSaveoption_golang" runat="server" Checked="true" Text="Lưu thông tin tương tự cho các ngôn ngữ khác?" />
                        </div>    
                    </fieldset>
                </div>
            </div>
        </div>
        
        </CONTENTTEMPLATE>
    </ASP:UPDATEPANEL>
    
    <script type="text/javascript">
    $(function() { initabs(); });
    
    function doSave() {
	    var the, txt = '', msg = '';
	    the = VALIDATA.GetObj('<%=txtName.ClientID %>');
        if(VALIDATA.IsNullOrEmpty(VALIDATA.GetVal(the))){
            txt = VALIDATA.Gettxt(txt, the);
            msg = VALIDATA.Getmsg(msg, the, VALIDATA.Error, Definephrase.Require_name);
            tabsForm.click(0);
        }
        the = VALIDATA.GetObj('<%=ddlCid.ClientID %>');
        if(VALIDATA.IsNullOrEmpty(VALIDATA.GetVal(the)) || VALIDATA.GetVal(the)==0){
            txt = VALIDATA.Gettxt(txt, the);
            msg = VALIDATA.Getmsg(msg, the, VALIDATA.Error, Definephrase.Require_catalogue);
            tabsForm.click(0);
        }
        the = VALIDATA.GetObj('<%=ddlName.ClientID %>');
        if(VALIDATA.IsNullOrEmpty(VALIDATA.GetVal(the)) || VALIDATA.GetVal(the)==0){
            txt = VALIDATA.Gettxt(txt, the);
            msg = VALIDATA.Getmsg(msg, the, VALIDATA.Error, Definephrase.Require_catalogue);
            tabsForm.click(0);
        }
	    
	    if(VALIDATA.Showerror(txt, msg)) return false;
	    return true;
    }

    </script>
    
</ASP:CONTENT>
