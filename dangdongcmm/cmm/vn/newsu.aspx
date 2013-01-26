<%@ Page Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="newsu.aspx.cs" Inherits="dangdongcmm.cmm.newsu" %>
<%@ MASTERTYPE VirtualPath="MasterDefault.Master" %>
<%@ REGISTER TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>
<%@ REGISTER TagPrefix="AjaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ REGISTER TagPrefix="UC" TagName="Assigntouser" Src="usercontrol/ucassigntouser.ascx" %>
<%@ REGISTER TagPrefix="UC" TagName="Filepreview" Src="usercontrol/ucfilepreview.ascx" %>
<%@ REGISTER TagPrefix="UC" TagName="Displaysetting" Src="usercontrol/ucdisplaysetting.ascx" %>
<%@ REGISTER TagPrefix="UC" TagName="Relatednews" Src="usercontrol/ucrelatednews.ascx" %>
<%@ REGISTER TagPrefix="UC" TagName="Categorymulti" Src="usercontrol/uccategorymulti.ascx" %>
<%@ REGISTER TagPrefix="UC" TagName="Categoryattr" Src="usercontrol/uccategoryattr.ascx" %>

<ASP:CONTENT ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript" src="../cssscript/jquery.ui.core.js"></script>
    <script type="text/javascript" src="../cssscript/jquery.ui.widget.js"></script>
    <script type="text/javascript" src="../cssscript/jquery.ui.datepicker.js"></script>
    <link type="text/css" rel="stylesheet" href="../cssscript/jquery.ui.css" />

	<script type="text/javascript">
	$(function() {
		$( "#<%=txtTimeexpire.ClientID %>" ).datepicker({
			showOn: "button",
			buttonImage: "../images/calendar.gif",
			dateFormat: "dd/mm/yy",
			showOtherMonths: true,
			selectOtherMonths: true,
			changeMonth: true,
			changeYear: true,
			buttonImageOnly: true
		});
	});
	</script>

    <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
        <CONTENTTEMPLATE>
        <ASP:LABEL ID="lblError" runat="server" CssClass="dformnoticeb"></ASP:LABEL>
        <ul class="tabs">
	        <li class="first"><a href="#">Thông Tin Chung</a></li>
	        <li><a href="#">Nội Dung</a></li>
	        <li><a href="#">Bài Viết Liên Quan</a></li>
	        <li><a href="#">Thiết Lập Hiển Thị</a></li>
        </ul>
        <div class="panes">
            <div class="tformin tformincom">
                <div class="rowhor">
                    <div class="title"><span class="require">*</span> Tên/ Tiêu đề:</div>
                    <div class="boxctrll"><ASP:TEXTBOX ID="txtName" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">Tác giả/ Nguồn trích:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="txtAuthor" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">Đường dẫn:</div>
                    <div class="boxctrll"><ASP:TEXTBOX ID="txtUrl" runat="server"></ASP:TEXTBOX></div>
                </div>
                <UC:FILEPREVIEW ID="Filepreview" runat="server" />
                <UC:CATEGORYMULTI ID="Categorymulti" runat="server" Belongto="12" Pid="0" />
                <div class="rowhor">
                    <div class="title"><span class="require">*</span> Danh mục mặc định:</div>
                    <div class="boxctrlc"><ASP:DROPDOWNLIST ID="ddlCid" runat="server"></ASP:DROPDOWNLIST></div>
                </div>
                <UC:CATEGORYATTR ID="Categoryattr" runat="server" Belongto="12" Pid="0" />
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
                <div class="rowhor">
                    <div class="title">Từ khóa:</div>
                    <div class="boxctrll"><ASP:TEXTBOX ID="txtTag" runat="server"></ASP:TEXTBOX></div>
                </div>
            </div>

            <div class="tformin tformincom">
                <UC:RELATEDNEWS ID="Relatednews" runat="server" />
            </div>
            
            <div class="tformin tformincom">
                <UC:DISPLAYSETTING ID="Displaysetting" runat="server" Showmarkas="true" />
                <div class="rowhor">
                    <div class="title"></div>
                    <div class="boxctrll"><ASP:CHECKBOX ID="chkAllowcomment" runat="server" Text="Mở: cho phép gửi phản hồi, bình luận" ToolTip="0" /></div>
                </div>
                <div class="rowhor">
                    <div class="title">Ngày hết hạn hiển thị:</div>
                    <div class="boxctrls"><ASP:TEXTBOX ID="txtTimeexpire" runat="server" Width="100px"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">&nbsp;</div>
                <UC:ASSIGNTOUSER ID="Assigntouser" runat="server" />
            </div>
        </div>
        
        <div class="tformin tformincom">
            <br />&nbsp;
            <div class="rowhor">
                <div class="title"></div>
                <div class="boxctrll">
                    <AJAXTOOLKIT:MASKEDEDITEXTENDER ID="MaskedEditExtender1" runat="server" TargetControlID="txtTimeexpire" Mask="99/99/9999" MaskType="Date" />
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
	    
	    if(VALIDATA.Showerror(txt, msg)) return false;
	    return true;
    }

    </script>

</ASP:CONTENT>
