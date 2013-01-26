<%@ Page Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="categoryattru.aspx.cs" Inherits="dangdongcmm.cmm.categoryattru" %>
<%@ MasterType virtualpath="MasterDefault.master" %>
<%@ REGISTER TagPrefix="UC" TagName="Filepreview" Src="usercontrol/ucfilepreview.ascx" %>
<%@ REGISTER TagPrefix="UC" TagName="Displaysetting" Src="usercontrol/ucdisplaysetting.ascx" %>
<%@ REGISTER TagPrefix="AjaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

<ASP:CONTENT ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
        <CONTENTTEMPLATE>
        <ASP:LABEL ID="lblError" runat="server" CssClass="dformnoticeb"></ASP:LABEL>
        <ul class="tabs">
	        <li class="first"><a href="#">Thông Tin Chung</a></li>
	        <li><a href="#">Thiết Lập Hiển Thị</a></li>
        </ul>
        <div class="panes">
            <div class="tformin tformincom">
                <div class="rowhor">
                    <div class="title"><span class="require">*</span> Loại danh mục:</div>
                    <div class="boxctrlm"><ASP:DROPDOWNLIST ID="ddlCid" runat="server"></ASP:DROPDOWNLIST></div>
                </div>
                <div class="rowhor">
                    <div class="title"><span class="require">*</span> Tên/ Tiêu đề:</div>
                    <div class="boxctrll"><ASP:TEXTBOX ID="txtName" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">Mã:</div>
                    <div class="boxctrls"><ASP:TEXTBOX ID="txtCode" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title"></div>
                    <div class="boxctrll"><ASP:CHECKBOX ID="chkPis" runat="server" Text="Có danh mục con?" ToolTip="0" /></div>
                </div>
                <div class="rowhor">
                    <div class="title">Đường dẫn liên kết:</div>
                    <div class="boxctrll"><ASP:TEXTBOX ID="txtUrl" runat="server"></ASP:TEXTBOX></div>
                </div>
                <UC:FILEPREVIEW ID="Filepreview" runat="server" Up="Li4vY29tbXVwL3VwbG9hZC8=" />
                <div class="rowhor">
                    <div class="title">Ghi chú:</div>
                    <div class="boxctrll"><ASP:TEXTBOX ID="txtNote" runat="server" TextMode="MultiLine" Rows="10"></ASP:TEXTBOX></div>
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
        the = VALIDATA.GetObj('<%=ddlCid.ClientID %>');
        if(VALIDATA.IsNullOrEmpty(VALIDATA.GetVal(the)) || VALIDATA.GetVal(the)==0){
            txt = VALIDATA.Gettxt(txt, the);
            msg = VALIDATA.Getmsg(msg, the, VALIDATA.Error, Definephrase.Require_catalogue);
        }
	    the = VALIDATA.GetObj('<%=txtName.ClientID %>');
        if(VALIDATA.IsNullOrEmpty(VALIDATA.GetVal(the))){
            txt = VALIDATA.Gettxt(txt, the);
            msg = VALIDATA.Getmsg(msg, the, VALIDATA.Error, Definephrase.Require_name);
        }
	    
	    if(VALIDATA.Showerror(txt, msg)) return false;
	    return true;
    }

    </script>    

</ASP:CONTENT>
