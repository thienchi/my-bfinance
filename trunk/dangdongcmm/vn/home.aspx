<%@ Page Language="C#" MasterPageFile="MasterHome.Master" AutoEventWireup="true" CodeBehind="home.aspx.cs" Inherits="dangdongcmm.home" %>
<%@ REGISTER TagPrefix="UC" TagName="Newsa" Src="usercontrol/ucnewsa.ascx" %>
<%@ REGISTER TagPrefix="UC" TagName="Newsadesc" Src="usercontrol/ucnewsadesc.ascx" %>
<%@ REGISTER TagPrefix="UC" TagName="Newsahome" Src="usercontrol/ucnewsahome.ascx" %>
<%@ REGISTER TagPrefix="UC" TagName="Homen" Src="usercontrol/uchomen.ascx" %>
<%@ REGISTER TagPrefix="UC" TagName="Focusn" Src="usercontrol/ucfocusn.ascx" %>
<%@ REGISTER TagPrefix="UC" TagName="Newsc" src="usercontrol/ucnewsc.ascx" %>
<%@ REGISTER TagPrefix="UC" TagName="Dictionaryc" src="usercontrol/ucdictionaryc.ascx" %>
<%@ REGISTER TagPrefix="UC" TagName="Searchdic" src="usercontrol/ucsearchdic.ascx" %>

<ASP:CONTENT ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="barthome1"><UC:NEWSAHOME ID="Newsahome" runat="server" Acode="'homeslide'" /></div>
    <div class="barthome2">
        <div class="fieldseti">
            <div class="cname2">Tiêu điểm</div>
            <div class="invitation"><UC:NEWSA ID="Newsa" runat="server" Acode="'invitation'" /></div>
        </div>
                   
    </div>
    
    <div class="barthome3">
        <div class="col1"><UC:NEWSADESC ID="Newsdesc1" runat="server" Acode="'home1'" /></div>
        <div class="col2"><UC:NEWSADESC ID="Newsdesc2" runat="server" Acode="'home2'" /></div>
    </div>
    
    <div class="barthome4">
        <UC:FOCUSN ID="Focusn" runat="server" />
        <UC:NEWSC ID="Newsc" runat="server" Categorycode="'quote'" />
    </div>
    
    <div class="barthome5">
        <div class="barhead">
            <div class="title"><a href="/tu-dien-thuat-ngu-abc-vn-at-.aspx">Từ điển thuật ngữ</a></div>
            <div class="listoption">Xem theo:&nbsp;&nbsp;&nbsp; <a href="/tu-dien-thuat-ngu-abc-vn-at-.aspx">ABC</a>
             &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp; <a href="/tu-dien-thuat-ngu-cat-vn-at-.aspx">Danh mục</a> &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
              <a href="tu-dien-thuat-ngu-abc-en-at-.aspx">Tiếng Anh</a></div>
            <div class="barsearchdic2"><UC:SEARCHDIC ID="Searchdic" runat="server" /></div>
        </div>
        <div class="col1">
            <div class="subject">Mới nhất</div>
            <div class="dicnewsest"><UC:DICTIONARYC ID="Dictionaryc1" runat="server" Categorycode="'dictionary'" SortExp="Id" PageSize="10" /></div>
        </div>
        <div class="col2">
            <div class="subject">Tra cứu nhiều nhất</div>
            <div class="dicviewest"><UC:DICTIONARYC ID="Dictionaryc2" runat="server" Categorycode="'dictionary'" SortExp="Viewcounter" PageSize="5" /></div>
        </div>
    </div>
    
    <div class="relatedl width100" style="margin-top:15px;"><UC:HOMEN ID="Homen" runat="server" /></div>
    
</ASP:CONTENT>
