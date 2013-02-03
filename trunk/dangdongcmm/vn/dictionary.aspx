<%@ Page Language="C#" MasterPageFile="MasterHome.Master" AutoEventWireup="true" CodeBehind="dictionary.aspx.cs" Inherits="dangdongcmm.dictionary" %>
<%@ MASTERTYPE virtualpath="MasterDefault.Master" %>
<%@ REGISTER TagPrefix="UC" TagName="Pager" Src="usercontrol/ucpager.ascx" %>
<%@ IMPORT Namespace="dangdongcmm.utilities" %>
<%@ REGISTER TagPrefix="UC" TagName="Cataloguemenu" Src="usercontrol/ucdictionarymenu.ascx" %>
<%@ REGISTER TagPrefix="UC" TagName="Comment" Src="usercontrol/uccomment.ascx" %>
<%@ REGISTER TagPrefix="UC" TagName="Newsrelated" Src="usercontrol/ucnewsrelated.ascx" %>
<%@ REGISTER TagPrefix="UC" TagName="Dictionaryc" src="usercontrol/ucdictionaryc.ascx" %>
<%@ REGISTER TagPrefix="UC" TagName="Searchdic" src="usercontrol/ucsearchdic.ascx" %>

<ASP:CONTENT ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript">
function EndRequestHandler(sender, args) {
    
}
</script>
    <div class="relatedl contentframe">
        <ASP:HIDDENFIELD ID="hidCidroot" runat="server" Value="61" />
        <div class="articleinmenu"><div class="first"><ASP:LABEL ID="lblCidrootname" runat="server"></ASP:LABEL></div></div>
        <ul class="tabs">
	        <li class="first"><a href="#">Xem theo ABC</a></li>
	        <li><a href="#">Xem theo danh mục</a></li>
        </ul>
        <ul class="langoption">
            <li><input type="checkbox" value="en" id="chkviewdicen" <%=this.Langview=="en" ? "checked=\"checked\"" : "" %> /> <label for="chkviewdicen">Tiếng Anh</label></li>
        </ul>
        <div class="panes">
            <div class="viewoption">
                <table cellpadding="5" cellspacing="0" class="tbabc">
                    <tr>
                        <td>0-9</td><td>A</td><td>B</td><td>C</td><td>D</td><td>E</td><td>F</td><td>G</td><td>H</td><td>I</td><td>J</td><td>K</td><td>L</td><td>M</td>
                    </tr>
                    <tr>
                        <td>N</td><td>O</td><td>P</td><td>Q</td><td>R</td><td>S</td><td>T</td><td>U</td><td>V</td><td>W</td><td>X</td><td>Y</td><td>Z</td><td></td>
                    </tr>
                </table>
            </div>
            <div class="viewoption">
                <div class="menudiccat"><UC:CATALOGUEMENU ID="Cataloguemenu" runat="server" Belongto="12" Cid='61' /></div>
            </div>
        </div>
        <script type="text/javascript">
        function initabs() {
	        $('ul.tabs').tabs('div.panes > div.viewoption');
	        var tabsForm = $('ul.tabs').data('tabs');
	        tabsForm.click(<%=this.Typeview == "abc" ? 0 : 1 %>);
        }
        $(function() { 
            initabs(); 
            var tb = $('.tbabc').find('td');
            tb.each(function(itd){
                tb.eq(itd).click(function(){
                    location.href = '/tu-dien-thuat-ngu-abc-<%=this.Langview %>-at-'+tb.eq(itd).html().toLowerCase()+'.aspx';
                });
            });            
            $('#chkviewdicen').change(function() {
                if(location.href.indexOf('-<%=this.Langview %>-') != -1) location.href = location.href.replace('-<%=this.Langview %>-', $(this).is(':checked') ? '-en-' : '-vn-');
                else location.href = location.href.replace('tu-dien-thuat-ngu-<%=this.Langview %>', $(this).is(':checked') ? 'tu-dien-thuat-ngu-en' : 'tu-dien-thuat-ngu-vn');
            });
          
        });        
        </script>
                
                <ASP:PANEL ID="pnlInfo" runat="server" CssClass="relatedl width100">
                    <ASP:REPEATER ID="rptInfo" runat="server">
                        <ITEMTEMPLATE>

                            <ASP:HIDDENFIELD ID="Id" runat="server" Value='<%#Eval("Id") %>' />
                            <div class="article">
                                <div class="name"><%#Eval("Name")%><%#Eval("Url").ToString() == "" ? "" : (" (" + Eval("Url") + ")")%> <%#Eval("eIconex")%></div>
                                <div class="desc"><%#Eval("Description")%></div>
                                <div align="right"><b><i><%#Eval("Author")%></i></b></div>
                                <UC:NEWSRELATED ID="Newsrelated" runat="server" Relateditem='<%#Eval("Relateditem")%>' />
                            </div>
                            <br />&nbsp;
                            <UC:COMMENT ID="Comment" runat="server" Allowcomment='<%#Eval("Allowcomment") %>' Belongto="12" Pid='<%#Eval("Id") %>' Pname='<%#Eval("Name") %>' />

                        </ITEMTEMPLATE>
                    </ASP:REPEATER>
                    
                </ASP:PANEL>
                
            <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
                <CONTENTTEMPLATE>
                
                <ASP:PANEL ID="pnlListfollow" runat="server" Visible="false" CssClass="relatedl width100">
                    <div class="title1bound">CÙNG DANH MỤC <ASP:LABEL ID="lblNamefollow" runat="server" Visible="false"></ASP:LABEL></div>
                    <ASP:REPEATER ID="rptListfollow" runat="server" >
                        <ITEMTEMPLATE>
                            <div class="article">
                                <div class="inlist">
                                 <a href="/tu-dien-thuat-ngu-vn/<%#Eval("eUrl2") %>"><%# this.Langview == "en" ?( Eval("Url") + "  ("+ Eval("Name").ToString() + ")") : (Eval("Name") + (Eval("Url").ToString() == "" ? "" : (" (" + Eval("Url") + ")")))%>
                                </a>

                                <%--<a href="d<%#Eval("Cid")%>d<%#Eval("Id")%>d=<%#Eval("eName")%>?pag=<%=PageIndex %>&ll=<%=Langview %>">                                
                                <%# this.Langview == "en" ?( Eval("Url") + "  ("+ Eval("Name").ToString() + ")") : (Eval("Name") + (Eval("Url").ToString() == "" ? "" : (" (" + Eval("Url") + ")")))%>
                                </a> <%#Eval("eIconex")%>--%>
                                </div>
                            </div>
                        </ITEMTEMPLATE>
                    </ASP:REPEATER>
                    <div align="left" class="pagercontainer" style="text-align:left;">
                        <UC:PAGER ID="pagBuilderfollow" runat="server" CssClass="pagBuilder" Infotemplate="<b>$RECORDCOUNT$</b> thuật ngữ" />
                    </div>
                </ASP:PANEL>
                
                <ASP:PANEL ID="pnlList" runat="server" Visible="false" CssClass="relatedl width100">
                    <ASP:REPEATER ID="rptList" runat="server">      
                        <ITEMTEMPLATE>
                            <div class="dictionaryl">
                                <div class="lname">
                                <a href="/tu-dien-thuat-ngu-vn/<%#Eval("eUrl2") %>"><%# this.Langview == "en" ?( Eval("Url") + "  ("+ Eval("Name").ToString() + ")") : (Eval("Name") + (Eval("Url").ToString() == "" ? "" : (" (" + Eval("Url") + ")")))%>
                                </a>
                               <%-- <a href="d<%#Eval("Cid")%>d<%#Eval("Id")%>d=<%#Eval("eName")%>?pag=<%=PageIndex %>&ll=<%=Langview %>">
                                <%# this.Langview == "en" ? (Eval("Url") + "  (" + Eval("Name").ToString() + ")") : (Eval("Name") + (Eval("Url").ToString() == "" ? "" : (" (" + Eval("Url") + ")")))%></a>
--%>                                
                                <%#Eval("eIconex") %>                                
                                </div>                                
                                <div class="intr"><%#Eval("Introduce") %>
                                </div>
                            </div>
                        </ITEMTEMPLATE>
                    </ASP:REPEATER>
                    <div align="right" class="pagercontainer">
                        <UC:PAGER ID="pagBuilder" runat="server" CssClass="pagBuilder" Infotemplate="<b>$RECORDCOUNT$</b> thuật ngữ" />
                    </div>
                </ASP:PANEL>
                
                </CONTENTTEMPLATE>
            </ASP:UPDATEPANEL>
    </div>    
    <script type="text/javascript">
    $(document).ready(function() {
        $(".dictionaryl:last").css("background","none");
        $("ul").find("li:last").css("font-weight","bold");
    });
    </script>    
    
    <div class="Righ2">
        <div class="fieldseti">
            <div class="cname">Danh mục</div>
            <div class="menudiccatright"><UC:CATALOGUEMENU ID="Cataloguemenu1" runat="server" Belongto="12" Cid='61' /></div>
        </div>
        <br />&nbsp;<br />&nbsp;
        <div class="barsearchdic"><UC:SEARCHDIC ID="Searchdic" runat="server" /></div>
        
        <div class="fieldsetf">
            <div class="cname">Tra cứu nhiều nhất</div>
            <div class="dicviewest"><UC:DICTIONARYC ID="Dictionaryc2" runat="server" Categorycode="'dictionary'" SortExp="Viewcounter" PageSize="10" /></div>
        </div>
    </div>
</ASP:CONTENT>
