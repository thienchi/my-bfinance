<%@ PAGE Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="menutypeofu.aspx.cs" Inherits="dangdongcmm.cmm.menutypeofu" %>
<%@ MASTERTYPE virtualpath="MasterDefault.master" %>
<%@ REGISTER Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolkit" %>

<ASP:CONTENT ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
        <CONTENTTEMPLATE>
        <ul class="tabs">
	        <li class="first"><a href="#">Thông Tin Chung</a></li>
	        <li><a href="#">Thiết Lập Hiển Thị</a></li>
        </ul>
        <div class="panes">
            <div class="tformin tformincom">
                <div class="rowhor">
                    <div class="title"><span class="require">*</span> Tên/ Tiêu đề:</div>
                    <div class="boxctrlm"><ASP:TEXTBOX ID="txtName" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">Mã:</div>
                    <div class="boxctrls"><ASP:TEXTBOX ID="txtCode" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">Ghi chú:</div>
                    <div class="boxctrlm"><ASP:TEXTBOX ID="txtNote" runat="server" TextMode="MultiLine" Rows="7"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title"><span class="require">*</span> Đường dẫn:</div>
                    <div class="boxctrlm"><ASP:TEXTBOX ID="txtPath" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title"></div>
                    <div class="boxctrlm"><ASP:CHECKBOX id="chkInsertbreak" runat="server" text="Chèn khoảng ngăn cách giữa các menu ?"></ASP:CHECKBOX></div>
                </div>
            </div>
            
            <div class="tformin tformincom">
                <div class="rowhor">
                    <div class="title">Trạng thái hiển thị:</div>
                    <div class="boxctrll">
                        <ASP:RADIOBUTTONLIST ID="radStatus" runat="server" RepeatDirection="Horizontal" CssClass="ctrlrad">
                            <ASP:LISTITEM Value="1" Selected="True">Hiển thị</ASP:LISTITEM>
                            <ASP:LISTITEM Value="4">Ẩn</ASP:LISTITEM>
                        </ASP:RADIOBUTTONLIST>
                    </div>
                </div>
            </div>
        </div>
                
        <div class="tformin tformincom">
            <br />&nbsp;
            <div class="rowhor">
                <div class="title">&nbsp;</div>
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
        }
    	
	    if(VALIDATA.Showerror(txt, msg)) return false;
	    return true;
    }

    </script> 
</ASP:CONTENT>
