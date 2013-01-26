<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucsearch.ascx.cs" Inherits="dangdongcmm.ucsearch" %>

<ASP:PANEL ID="pnlSearch" runat="server">
    <div class="SEARCHPANEL">
        <div class="belongto">
            <ASP:CHECKBOXLIST Visible="false" ID="chkBelongto" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="chkBelongto_SelectedIndexChanged" AutoPostBack="true">
                <ASP:LISTITEM Value="12" Selected="True">Tin tức</ASP:LISTITEM>
            </ASP:CHECKBOXLIST>
        </div>
        <div class="categories"><ASP:DROPDOWNLIST Visible="false" ID="ddlCid" runat="server"></ASP:DROPDOWNLIST></div>
        <div class="keywords"><ASP:TEXTBOX ID="txtKeywords" runat="server" Text="Nhập từ khóa..."></ASP:TEXTBOX></div>
        <div class="command"><ASP:IMAGEBUTTON ID="cmdSearch" runat="server" ImageUrl="../../images/search-button.png" OnClientClick="javascript:return doSearch();" OnClick="cmdSearch_Click" /></div>
    </div>
   <ASP:HIDDENFIELD ID="LevelOfCid" runat="server" Value="0" />
   <ASP:HIDDENFIELD ID="RootOfCid" runat="server" Value="0" />
    <script type="text/javascript">
    function doSearch() {
        if($('#<%=txtKeywords.ClientID %>').val()=='' || $('#<%=txtKeywords.ClientID %>').val()=='Nhập từ khóa...'){
            $('#<%=txtKeywords.ClientID %>').focus();
            return false;
        }
        $('#<%=txtKeywords.ClientID %>').val('"' + $('#<%=txtKeywords.ClientID %>').val() + '"');
        prompt_processing();
        return true;
    }
    
    $('#<%=txtKeywords.ClientID %>').focus(function(){
        if($('#<%=txtKeywords.ClientID %>').val()=='Nhập từ khóa...')
            $('#<%=txtKeywords.ClientID %>').val('');
    });
    $('#<%=txtKeywords.ClientID %>').blur(function(){
        if($('#<%=txtKeywords.ClientID %>').val()=='')
            $('#<%=txtKeywords.ClientID %>').val('Nhập từ khóa...');
    });
    </script>

</ASP:PANEL>