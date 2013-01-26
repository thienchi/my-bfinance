<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucnewsf.ascx.cs" Inherits="dangdongcmm.ucnewsf" %>
<%@ REGISTER TagPrefix="UC" TagName="Pager" Src="ucpager.ascx" %>

<ASP:PANEL ID="pnlList" runat="server" CssClass="fieldsetf">
    <ASP:REPEATER ID="rptList" runat="server">
        <ITEMTEMPLATE>
            <div class="cname"><%#Eval("Cname") %></div>
            <div class="newsquote">
                <div class="lname"><a href="<%#Eval("eUrl") %>"><%#Eval("Name")%></a> <%#Eval("eIconex") %></div>
            </div>
        </ITEMTEMPLATE>
    </ASP:REPEATER>
				
    <UC:PAGER ID="pagBuilder" runat="server" CssClass="pagBuilder" Infotemplate="" PageSize="2" SortDir="Random" Visible="false" />

</ASP:PANEL>
