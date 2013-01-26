<%@ Page Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="staticcontentu.aspx.cs" Inherits="dangdongcmm.cmm.staticcontentu" %>
<%@ MASTERTYPE VirtualPath="MasterDefault.Master" %>
<%@ REGISTER TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>

<ASP:CONTENT ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
        <CONTENTTEMPLATE>
        <ASP:LABEL ID="lblError" runat="server" CssClass="dformnoticeb"></ASP:LABEL>
        <ul class="tabs">
	        <li class="first"><a href="#">Thông Tin Chung</a></li>
	        <li><a href="#">Nội Dung</a></li>
        </ul>
        <div class="panes">
            <div class="tformin tformincom">
                <div class="rowhor">
                    <div class="title"><span class="require">*</span> Tên/ Tiêu đề:</div>
                    <div class="boxctrll"><ASP:TEXTBOX ID="txtName" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">Mã:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="txtCode" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title"></div>
                    <div class="boxctrll"><ASP:CHECKBOX ID="chkSeparatepage" runat="server" Checked="true" Text="Hiển thị nội dung này trên một trang riêng?" /></div>
                </div>
                <div class="rowhor">
                    <div class="title">Tên tập tin:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="txtFilepath" runat="server"></ASP:TEXTBOX></div>
                </div>
            </div>
            <div class="tformin tformincom">
                <div class="rowhor">
                    <div class="title">Nội dung:</div>
                    <div class="boxctrll"><FTB:FREETEXTBOX id="txtDescription" runat="Server" width="100%"
								    buttonset="Office2003" toolbarlayout="FontFacesMenu, FontSizesMenu, FontForeColorsMenu , FontForeColorPicker | Bold, Italic, Underline, Strikethrough; Superscript, Subscript, RemoveFormat | JustifyLeft, JustifyRight, JustifyCenter, JustifyFull, BulletedList; NumberedList, Indent, Outdent; CreateLink, Unlink, InsertImage, InsertRule, InsertDiv | Cut, Copy, Paste; Undo, Redo, Print"
								    height="400px"></FTB:FREETEXTBOX></div>
                </div>
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
        the = VALIDATA.GetObj('<%=chkSeparatepage.ClientID %>');
        if(the.attr('checked')==false) {
	        the = VALIDATA.GetObj('<%=txtFilepath.ClientID %>');
            if(VALIDATA.IsNullOrEmpty(VALIDATA.GetVal(the))){
                txt = VALIDATA.Gettxt(txt, the);
                msg = VALIDATA.Getmsg(msg, the, VALIDATA.Error, Definephrase.Invalid_file);
                tabsForm.click(0);
            }
        }
        
	    if(VALIDATA.Showerror(txt, msg)) return false;
	    return true;
    }

    </script>
    
</ASP:CONTENT>
