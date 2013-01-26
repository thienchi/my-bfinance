<%@ Page Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="usergroupu.aspx.cs" Inherits="dangdongcmm.cmm.usergroupu" %>
<%@ MasterType virtualpath="MasterDefault.master" %>

<ASP:CONTENT ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
        <CONTENTTEMPLATE>
        <ASP:LABEL ID="lblError" runat="server" CssClass="dformnoticeb"></ASP:LABEL>
        <ASP:PANEL ID="pnlForm" runat="server">
        <ul class="tabs">
	        <li class="first"><a href="#">Thông tin nhóm</a></li>
	        <li><a href="#">Các quyền của nhóm</a></li>
        </ul>
        <div class="panes">
            <div class="tformin tformincom">
                <div class="rowhor">
                    <div class="title"><span class="require">*</span> Tên nhóm:</div>
                    <div class="boxctrlm"><ASP:TEXTBOX ID="txtName" runat="server"></ASP:TEXTBOX></div>
                </div>
            </div>

            <div class="tformin tformincom">
                <div class="rowhor">
                    <div class="title"></div>
                    <div class="boxctrll"><b>Quyền chỉnh sửa, cập nhật dữ liệu trên các trang sau:</b></div>
                </div>
                <div class="rowhor">
                    <div class="title"></div>
                    <div class="boxctrll"><ASP:CHECKBOX ID="chkr_sys" runat="server" Text="Quản trị tối cao"></ASP:CHECKBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title"></div>
                    <div class="boxctrll">
                        <ASP:DATALIST ID="dtlListRPages" runat="server" CssClass="ldatalist" Width="100%" DataKeyField="Navigateurl" 
                            OnItemDataBound="dtlListRPages_ItemDataBound">
                            <ITEMTEMPLATE>
                            <div style="border-bottom:1px dotted #ccc; padding-bottom:5px;">
                                <ASP:CHECKBOX ID="RPages_typeof" runat="server" Text='<%#Eval("Name") %>' />
                                <ASP:PANEL ID="divRPages" runat="server">
                                    <ASP:LISTBOX ID="RPages_cid" runat="server" SelectionMode="Multiple" Height="120px"></ASP:LISTBOX>
                                </ASP:PANEL>
                            </div>    
                            </ITEMTEMPLATE>
                        </ASP:DATALIST>
                        <script type="text/javascript">
                        function toggleRPages(div){
                            $("#" + div).toggle();
                        }
                        </script>
                    </div>
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
                        </div>    
                    </fieldset>
                </div>
            </div>
        </div>
        
        </ASP:PANEL>
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
