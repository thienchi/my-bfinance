<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucsearch.ascx.cs" Inherits="dangdongcmm.ucsearch" %>

<ASP:PANEL ID="pnlSearch" runat="server" DefaultButton="cmdSearch">
    <div class="SEARCHDIC">
        <ASP:CHECKBOXLIST Visible="false" ID="chkBelongto" runat="server"><ASP:LISTITEM Value="12" Selected="True">Tin tức</ASP:LISTITEM></ASP:CHECKBOXLIST>
        <div class="categories"><ASP:DROPDOWNLIST Visible="false" ID="ddlCid" runat="server"></ASP:DROPDOWNLIST></div>
        <div class="keywords"><ASP:TEXTBOX ID="txtKeywords" runat="server" Text="Tra cứu nhanh..."></ASP:TEXTBOX></div>
        <div class="command"><ASP:IMAGEBUTTON ID="cmdSearch" runat="server" ImageUrl="../../images/searchdic-button.png" OnClientClick="javascript:return doSearchdic();" /></div>
    </div>
   <ASP:HIDDENFIELD ID="LevelOfCid" runat="server" Value="2" />
   <ASP:HIDDENFIELD ID="RootOfCid" runat="server" Value="61" />
    <script type="text/javascript">
    function doSearchdic() {
        if($('#<%=txtKeywords.ClientID %>').val()=='' || $('#<%=txtKeywords.ClientID %>').val()=='Tra cứu nhanh...'){
            $('#<%=txtKeywords.ClientID %>').focus();
            return false;
        }
        //$('#<%=txtKeywords.ClientID %>').val('"' + $('#<%=txtKeywords.ClientID %>').val() + '"');
        prompt_processing();
        location.href = 'dictionary-search-vn-at-' + $('#<%=txtKeywords.ClientID %>').val() + '.aspx';
        return false;
    }
    
    $('#<%=txtKeywords.ClientID %>').focus(function(){
        if($('#<%=txtKeywords.ClientID %>').val()=='Tra cứu nhanh...')
            $('#<%=txtKeywords.ClientID %>').val('');
    });
    $('#<%=txtKeywords.ClientID %>').blur(function(){
        if($('#<%=txtKeywords.ClientID %>').val()=='')
            $('#<%=txtKeywords.ClientID %>').val('Tra cứu nhanh...');
    });
    </script>

</ASP:PANEL>