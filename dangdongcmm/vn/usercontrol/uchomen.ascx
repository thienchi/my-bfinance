<%@ Control Language="C#" AutoEventWireup="true" Inherits="dangdongcmm.uchomen" %>
<%@ REGISTER TagPrefix="UC" TagName="Newsh" Src="ucnewsh.ascx" %>

<ASP:PANEL ID="pnlList" runat="server" CssClass="relatedl width100">
<table border="0" cellpadding="0" cellspacing="0" width="100%" class="tbartselhome">
    <tr>
    <ASP:REPEATER ID="rptList" runat="server">
        <ITEMTEMPLATE>
        <td width="50%" valign="top" style="padding-bottom:15px; <%#(Container.ItemIndex % 2 == 0) ? "padding-right:15px;" : "border-left:1px solid #ccc; padding-left:15px" %>">
            <div class="cname"><a href="/<%#Eval("eUrl2").ToString()%>"><%#Eval("Name") %></a></div>
            <UC:NEWSH ID="Newsh" runat="server" Categoryid='<%#Eval("Id") %>' />
        </td>
        <%#((Container.ItemIndex + 1) % 2 == 0) ? "</tr><tr>" : ""%>
        </ITEMTEMPLATE>
    </ASP:REPEATER>
</table>
</ASP:PANEL>