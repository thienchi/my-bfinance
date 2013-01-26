<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucfocusn.ascx.cs" Inherits="dangdongcmm.ucfocusn" %>

<ASP:PANEL ID="pnlList" runat="server" CssClass="thematic">

    <div class="fieldsetf">
        <div class="cname">Chuyên đề</div>
        <ASP:REPEATER ID="rptList" runat="server">
            <ITEMTEMPLATE>
                <div class="thematicinfo">
                    <div class="lname"><a href="<%#Eval("eUrl") %>"><%#Eval("Name")%></a> <%#Eval("eIconex") %> <div class="stt"><%#Container.ItemIndex + 1 %></div></div>
                </div>
            </ITEMTEMPLATE>
        </ASP:REPEATER>
    </div>
    <script type="text/javascript">
    $(document).ready(function() {
        $(".thematicinfo:last").css("border","none");
        $(".newsadesc:last").css("background","none");
        $(".article:last").css("background","none");
    });
    </script>

</ASP:PANEL>
