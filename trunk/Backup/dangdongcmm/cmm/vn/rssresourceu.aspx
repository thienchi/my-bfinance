<%@ Page Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="rssresourceu.aspx.cs" Inherits="dangdongcmm.cmm.rssresourceu" %>
<%@ MasterType virtualpath="MasterDefault.master" %>
<%@ REGISTER TagPrefix="AjaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ REGISTER TagPrefix="UC" TagName="Displaysetting" Src="usercontrol/ucdisplaysetting.ascx" %>

<ASP:CONTENT ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
        <CONTENTTEMPLATE>
        <ASP:LABEL ID="lblError" runat="server" CssClass="dformnoticeb"></ASP:LABEL>
        <ul class="tabs">
	        <li class="first"><a href="#">Thông Tin Chung</a></li>
	        <li><a href="#">Nhận Dạng Nội Dung Bài Viết</a></li>
	        <li><a href="#">Thiết Lập Hiển Thị</a></li>
        </ul>
        <div class="panes">
            <div class="tformin tformincom">
                <div class="rowhor">
                    <div class="title"><span class="require">*</span> Tên / Tiêu đề:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="txtName" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title"><span class="require">*</span> Website nguồn:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="txtWebsiteUrl" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title"><span class="require">*</span> Đường dẫn RSS:</div>
                    <div class="boxctrlm"><ASP:TEXTBOX ID="txtRSSUrl" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title"><span class="require">*</span> Đưa tin vào danh mục:</div>
                    <div class="boxctrlc"><ASP:DROPDOWNLIST ID="ddlCid" runat="server"></ASP:DROPDOWNLIST></div>
                </div>
                <div class="rowhor">
                    <div class="title">Lần lấy tin cuối cùng:</div>
                    <div class="boxctrll"><ASP:LABEL ID="lblTimelastestget" runat="server" Text="n/a"></ASP:LABEL></div>
                </div>
            </div>
            
            <div class="tformin tformincom">
                <div class="rowhor">
                    <div class="title">Nhận dạng khung hiển thị bài viết:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="txtNodecontent" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">Nhận dạng tiêu đề:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="txtNodetitle" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">Nhận dạng phần giới thiệu:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="txtNodeintroduce" runat="server"></ASP:TEXTBOX></div>
                </div>
            </div>
            
            <div class="tformin tformincom">
                <UC:DISPLAYSETTING ID="Displaysetting" runat="server" Showicon="false" Showdisplayorder="false" />
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
	    
	    if(VALIDATA.Showerror(txt, msg)) return false;
	    return true;
    }

    </script>

</ASP:CONTENT>
