<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucnewsa.ascx.cs" Inherits="dangdongcmm.ucnewsa" %>
<%@ REGISTER TagPrefix="UC" TagName="Pager" Src="ucpager.ascx" %>

<ASP:PANEL ID="pnlList" runat="server" CssClass="bannerbar">
    <div id="flowpanes">
        <div class="items">
    <ASP:REPEATER ID="rptList" runat="server">
        <ITEMTEMPLATE>
            <div class="newsf">
                <div class="blockimg"><a href="/<%#Eval("eUrl2") %>"><%#Eval("eFilepreview") %></a></div>
                <div class="cname"><%#Eval("Cname") %></div>
                <div class="lname"><a href="/<%#Eval("eUrl2") %>"><%#Eval("Name")%></a> <%#Eval("eIconex") %></div>
                <%--<div class="intr"><%#Eval("Introduce")%> <a class="more" href="/<%#Eval("eUrl2") %>?pag=<%=PageIndex %>">...chi tiết</a></div>--%>
                <div class="intr"><%#Eval("Introduce")%> <a class="more" href="/<%#Eval("eUrl2") %>">  ...chi tiết</a></div>
                <div class="newsfupnext" id="upnext<%#Container.ItemIndex %>"><a href="/<%#Eval("eUrl2") %>"><%#Eval("Name").ToString().Length > 60 ? Eval("Name").ToString().Substring(0, 60) + "..." : Eval("Name").ToString()%></a></div>
            </div>
        </ITEMTEMPLATE>
    </ASP:REPEATER>
        </div>
    </div>
    <div class="flowupnext">Tiếp theo:</div>
	<div id="flowtabs">
		<a href="#" class="first">1</a>
		<a href="#">2</a>
		<a href="#">3</a>
	</div>
				
    <UC:PAGER ID="pagBuilder" runat="server" CssClass="pagBuilder" Infotemplate="" PageSize="3" Visible="false" />

    <script type="text/javascript">
        $(function() {
            var temp;
            var des = $("#flowpanes").find("div[id^='upnext']");
            des.each(function (di) {
                if(di==0) temp = des.eq(0).html();
                if(di<2) des.eq(di).html(des.eq(di+1).html());
                else des.eq(di).html(temp);
            });
        
            $("#flowpanes").scrollable({ circular: true }).autoscroll({ interval: 15000, autoplay: true }).navigator({
                navi: "#flowtabs",
                naviItem: 'a',
                activeClass: 'current',
                history: false
            });
            var instance = $("#flowpanes").data("scrollable").seekTo(Math.floor((Math.random()*3)), 0);
        });
        
    </script>

</ASP:PANEL>