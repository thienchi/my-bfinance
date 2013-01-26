<%@ PAGE Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="rssresourcel.aspx.cs" Inherits="dangdongcmm.cmm.rssresourcel" %>
<%@ MASTERTYPE virtualpath="MasterDefault.master" %>
<%@ REGISTER TagPrefix="UC" TagName="Pager" Src="usercontrol/ucpager.ascx" %>
<%@ REGISTER TagPrefix="UC" TagName="Setupattribute" Src="usercontrol/ucsetupattribute.ascx" %>

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
                    <ASP:BUTTON ID="cmdAdd" runat="server" Text="Thêm mới" CssClass="buttoncmd" />&nbsp;
                    -- hoặc --
                    <ASP:DROPDOWNLIST ID="ddlAction" runat="server" onchange="actionMulti(this);" OnSelectedIndexChanged="ddlAction_SelectedIndexChanged">
                        <ASP:LISTITEM Value="" class="textdefndis">Thiết lập cho nhiều tin...</ASP:LISTITEM>
                        <ASP:LISTITEM Value="Delete">-- Xóa</ASP:LISTITEM>
                        <ASP:LISTITEM Value="Setupattribute">-- Cài đặt hiển thị</ASP:LISTITEM>
                    </ASP:DROPDOWNLIST>
                </div>  
            </div>
            
            <div class="dformoutlist">
            <ASP:GRIDVIEW ID="grdView" runat="server" DataKeyNames="Id" CssClass="lgridview" 
                AutoGenerateColumns="false" AllowSorting="true" AllowPaging="false" 
                OnRowDataBound="grdView_RowDataBound" OnSorting="grdView_Sorting" OnRowDeleting="grdView_RowDeleting" OnRowCommand="grdView_RowCommand">
                <COLUMNS>
                    <ASP:TEMPLATEFIELD HeaderText="#" HeaderStyle-CssClass="colnum" ItemStyle-CssClass="colnum">
                        <ITEMTEMPLATE>
                            <%# Eval("Rownumber")%>
                            <ASP:HIDDENFIELD ID="txtId" runat="server" Value='<%#Eval("Id") %>' />
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:TEMPLATEFIELD itemstyle-cssclass="colcheck">
                        <HEADERTEMPLATE>
                            <input type="checkbox" id="chkcheckall" onclick="tglSel('chkcheckall', 'chkcheck');" />
                        </HEADERTEMPLATE>
                        <ITEMTEMPLATE>
                            <input type="checkbox" id="chkcheck<%# Eval("Id")%>" name="chkcheck" value="<%# Eval("Id")%>" onclick="chkAll('chkcheckall', this);" />
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:TEMPLATEFIELD headertext="Tên / Tiêu đề" sortexpression="Name">
                        <ITEMTEMPLATE>
                            <ASP:LINKBUTTON ID="cmdSetupattribute" runat="server" CommandName="Setupattribute" CommandArgument='<%# Eval("Id")%>' Text='<%#Eval("Name") %>' CausesValidation="false"></ASP:LINKBUTTON> <%#Eval("Iconex").ToString() == "" ? "" : "<img src='../../" + Eval("Iconex") + "' />" %>
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:TEMPLATEFIELD headertext="Tin hôm nay" itemstyle-cssclass="colstatus">
                        <ITEMTEMPLATE>
                            <a href="rssquery.aspx?iid=<%#Eval("Id") %>">Truy vấn tin</a>
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:TEMPLATEFIELD headertext="Tin đã lưu lúc" itemstyle-cssclass="colpis">
                        <ITEMTEMPLATE>
                            <%#Eval("eTimelastestget") %>
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:BOUNDFIELD datafield="eStatus" headertext="Trạng thái" sortexpression="Status" itemstyle-cssclass="colstatus" HtmlEncode="false"></ASP:BOUNDFIELD>
                    <ASP:TEMPLATEFIELD headertext="Cập nhật" sortexpression="Timeupdate" itemstyle-cssclass="colupdate">
                        <ITEMTEMPLATE>
                            <div><%#Eval("Username") %></div>
                            <div><%#Eval("eTimeupdate") %></div>
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:TEMPLATEFIELD itemstyle-cssclass="colaction">
                        <ITEMTEMPLATE>
                            <a class="command" href="javascript:CC_gotoUrl('rssresourceu.aspx?iid=<%#Eval("Id") %>');">Sửa</a>
                            &nbsp;|&nbsp;
                            <ASP:LINKBUTTON ID="cmdDeleteone" runat="server" CssClass="command" Text="Xóa" CommandName="Delete" CausesValidation="false"></ASP:LINKBUTTON>
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
            <UC:SETUPATTRIBUTE ID="Setupattribute" runat="server" />
        </ASP:PANEL>
        
        </CONTENTTEMPLATE>
    </ASP:UPDATEPANEL>
    
</ASP:CONTENT>
