<%@ PAGE Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="waittingdelete.aspx.cs" Inherits="dangdongcmm.cmm.waittingdelete" %>
<%@ MASTERTYPE virtualpath="MasterDefault.master" %>
<%@ REGISTER TagPrefix="UC" TagName="Pager" Src="usercontrol/ucpager.ascx" %>

<ASP:CONTENT ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
        <CONTENTTEMPLATE>
        
        <ASP:LABEL ID="lblError" runat="server" CssClass="dformnoticeb"></ASP:LABEL>
        <ASP:PANEL id="pnlList" runat="server" width="100%">
            <div class="dformoutcommand">
                <div class="colfilter">
                    <ASP:PANEL ID="panelSearch" runat="server" DefaultButton="cmdSearch">
                        <table cellspacing="0" cellpadding="0" border="0">
                            <tr>
                                <td><img src="/cmm/images/symsearch.gif" alt="" /></td>
                                <td><ASP:TEXTBOX ID="txtKeyword" runat="server"></ASP:TEXTBOX></td>
                                <td><ASP:BUTTON ID="cmdSearch" runat="server" OnClientClick="javascript:return doSubmit();" OnClick="cmdSearch_Click" CssClass="button" Text="Search" /></td>
                            </tr>
                        </table>
                    </ASP:PANEL>
                </div>
                <div class="colsetting">
                    <ASP:BUTTON ID="cmdAccept" runat="server" OnClientClick="javascript:return doSubmitSel();" OnClick="cmdAccept_Click" Text="Phục hồi thông tin" CssClass="buttoncmd" />&nbsp;
                    -- hoặc --
                    <ASP:BUTTON ID="cmdDelete" runat="server" OnClientClick="javascript:return doSubmitDel();" OnClick="cmdDelete_Click" Text="Xóa" CssClass="buttoncmd" />&nbsp;
                </div>  
            </div>
            
            <div class="dformoutlist">
            <ASP:GRIDVIEW ID="grdView" runat="server" DataKeyNames="Id,Belongto" CssClass="lgridview" 
                AutoGenerateColumns="false" AllowSorting="true" AllowPaging="false" 
                OnRowDataBound="grdView_RowDataBound" OnSorting="grdView_Sorting" OnRowDeleting="grdView_RowDeleting" OnRowUpdating="grdView_RowUpdating">
                <COLUMNS>
                    <ASP:TEMPLATEFIELD HeaderText="#" HeaderStyle-CssClass="colnum" ItemStyle-CssClass="colnum">
                        <ITEMTEMPLATE>
                            &nbsp;
                            <ASP:HIDDENFIELD ID="txtId" runat="server" Value='<%#Eval("Id") %>' />
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:TEMPLATEFIELD itemstyle-cssclass="colcheck">
                        <HEADERTEMPLATE>
                            <input type="checkbox" id="chkcheckall" onclick="tglSel('chkcheckall', 'chkcheck');" />
                        </HEADERTEMPLATE>
                        <ITEMTEMPLATE>
                            <input type="checkbox" id="chkcheck<%# Eval("Id")%>" name="chkcheck" value="<%# Eval("Id")%>:<%# Eval("Belongto")%>" onclick="chkAll('chkcheckall', this);" />
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:TEMPLATEFIELD headertext="Tên/ Tiêu đề">
                        <ITEMTEMPLATE>
                            <a><%#Eval("Name") %></a>
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:BOUNDFIELD datafield="Belongtoname" headertext="Loại thông tin" itemstyle-cssclass="colinfo2"></ASP:BOUNDFIELD>
                    <ASP:BOUNDFIELD datafield="eStatus" headertext="Trạng thái" itemstyle-cssclass="colinfo1" HtmlEncode="false"></ASP:BOUNDFIELD>
                    <ASP:TEMPLATEFIELD headertext="Cập nhật" itemstyle-cssclass="colupdate">
                        <ITEMTEMPLATE>
                            <div><%#Eval("Username") %></div>
                            <div><%#Eval("eTimeupdate") %></div>
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:TEMPLATEFIELD itemstyle-cssclass="colinfo2">
                        <ITEMTEMPLATE>
                            <ASP:LINKBUTTON ID="lnkDelete" runat="server" CssClass="command" Text="Xóa" CommandName="Delete" CommandArgument='<%#Eval("Belongto") %>' CausesValidation="false"></ASP:LINKBUTTON>
                            &nbsp;|&nbsp;
                            <ASP:LINKBUTTON ID="lnkAccept" runat="server" CssClass="command" Text="Phục hồi" CommandName="Update" CommandArgument='<%#Eval("Belongto") %>' CausesValidation="false"></ASP:LINKBUTTON>
                            &nbsp;|&nbsp;
                            <ASP:LINKBUTTON ID="lnkEdit" runat="server" CssClass="command" Text="Sửa"></ASP:LINKBUTTON>
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                </COLUMNS>
                <HEADERSTYLE cssclass="rowheader" />
                <ROWSTYLE cssclass="rowormal" />
                <ALTERNATINGROWSTYLE cssclass="rowalter" />
                
            </ASP:GRIDVIEW>
            <div align="right" class="pagercontainer">
		        <UC:PAGER ID="pagBuilder" runat="server" CssClass="pagBuilder" Infotemplate="" />
            </div>
            </div>
        </ASP:PANEL>
        
        </CONTENTTEMPLATE>
    </ASP:UPDATEPANEL>
    
</ASP:CONTENT>
