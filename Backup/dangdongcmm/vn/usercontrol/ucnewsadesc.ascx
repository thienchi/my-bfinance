<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucnewsa.ascx.cs" Inherits="dangdongcmm.ucnewsa" %>
<%@ REGISTER TagPrefix="UC" TagName="Pager" Src="ucpager.ascx" %>

<ASP:PANEL ID="pnlList" runat="server" CssClass="relatedl width100">
    <ASP:REPEATER ID="rptList" runat="server">
        <ITEMTEMPLATE>
            <div class="newsadesc">
                <div class="lname"><a href="<%#Eval("eUrl") %>"><%#Eval("Name")%></a> <%#Eval("eIconex")%></div>
                <%#Eval("eFilepreview").ToString() == "" ? "" : ("<div class=\"blockimg\"><a href=\"" + Eval("eUrl") + "\">" + Eval("eFilepreview") + "</a></div>")%>
                <div class="intr"><%#Eval("Introduce")%></div>
            </div>
        </ITEMTEMPLATE>
    </ASP:REPEATER>
    <UC:PAGER ID="pagBuilder" runat="server" CssClass="pagBuilder" Infotemplate="" Visible="false" PageSize="3" />
</ASP:PANEL>