<%@ PAGE Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="categorytypeofu.aspx.cs" Inherits="dangdongcmm.cmm.categorytypeofu" %>
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
                    <div class="title"><span class="require">*</span> ID:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="txtId" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">Mã:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="txtCode" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">Ghi chú:</div>
                    <div class="boxctrlm"><ASP:TEXTBOX ID="txtNote" runat="server" Width="99%" TextMode="MultiLine" Rows="7"></ASP:TEXTBOX></div>
                </div>
            </div>
        
            <div class="tformin tformincom">
                <div class="rowhor">
                    <div class="title">Trạng thái:</div>
                    <div class="boxctrll">
                        <ASP:RADIOBUTTONLIST ID="radStatus" runat="server" RepeatDirection="Horizontal" CssClass="ctrlrad">
                            <ASP:LISTITEM Value="1" Selected="True">Hiển thị</ASP:LISTITEM>
                            <ASP:LISTITEM Value="4">Ẩn</ASP:LISTITEM>
                        </ASP:RADIOBUTTONLIST>
                    </div>
                </div>
                <div class="rowhor">
                    <div class="title">Thiết lập hiển thị:</div>
                    <div class="boxctrll">
                        <ASP:RADIOBUTTONLIST ID="radMarkas" runat="server" RepeatDirection="Horizontal" CssClass="ctrlrad">
                            <ASP:LISTITEM Value="0">Không</ASP:LISTITEM>
                            <ASP:LISTITEM Value="1">Trên trang chủ <img src="../images/icon_home.gif" /></ASP:LISTITEM>
                            <ASP:LISTITEM Value="2">Nổi bật, tiêu điểm <img src="../images/icon_focus.gif" /></ASP:LISTITEM>
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
                    <AJAXTOOLKIT:FILTEREDTEXTBOXEXTENDER ID="FilteredTextBoxExtender1" runat="server" TargetControlID="txtId" FilterType="Numbers" />
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
        the = VALIDATA.GetObj('<%=txtId.ClientID %>');
        if(VALIDATA.IsNullOrEmpty(VALIDATA.GetVal(the)) || !VALIDATA.IsNumber(VALIDATA.GetVal(the))){
            txt = VALIDATA.Gettxt(txt, the);
            msg = VALIDATA.Getmsg(msg, the, VALIDATA.Error, Definephrase.Invalid_int);
        }
    	
	    if(VALIDATA.Showerror(txt, msg)) return false;
	    return true;
    }

    </script>

</ASP:CONTENT>
