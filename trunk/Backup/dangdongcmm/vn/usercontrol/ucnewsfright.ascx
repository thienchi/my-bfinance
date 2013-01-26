<%@ Control Language="C#" AutoEventWireup="true" Inherits="dangdongcmm.ucnewsf" %>
<%@ REGISTER TagPrefix="UC" TagName="Pager" Src="ucpager.ascx" %>
<%@ REGISTER TagPrefix="UC" TagName="Newsadesc" Src="ucnewsaright.ascx" %>

<ASP:PANEL ID="pnlList" runat="server" CssClass="fieldseti">
    <div class="cname">Nổi bật</div>
    <div id="focuslist" class="relatedl width100">
    <ASP:REPEATER ID="rptList" runat="server">
        <ITEMTEMPLATE>
            <div class="newsadesc" id="newsadesc<%#Eval("Id")%>">
                <%#Eval("eFilepreview").ToString() == "" ? "" : ("<div class=\"blockimg\"><a href=\"" + Eval("eUrl") + "\">" + Eval("eFilepreview") + "</a></div>")%>
                <div class="lname"><a href="<%#Eval("eUrl") %>"><%#Eval("Name")%></a> <%#Eval("eIconex")%></div>
            </div>
        </ITEMTEMPLATE>
    </ASP:REPEATER>
    <UC:NEWSADESC ID="Newsdesc1" runat="server" Acode="'home1'" />
    <UC:NEWSADESC ID="Newsdesc2" runat="server" Acode="'home2'" />
    </div>
    <UC:PAGER ID="pagBuilder" runat="server" CssClass="pagBuilder" Infotemplate="" PageSize="6" Visible="false" />

</ASP:PANEL>