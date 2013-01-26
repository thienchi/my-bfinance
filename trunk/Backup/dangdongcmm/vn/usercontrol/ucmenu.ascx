<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucmenu.ascx.cs" Inherits="dangdongcmm.ucmenu" %>

<ASP:LITERAL ID="ltrMenumain" runat="server"></ASP:LITERAL>

<script type="text/javascript">
$(document).ready(function() {
    var pagename = location.href.substring(location.href.lastIndexOf('/') + 1, location.href.indexOf('.aspx') + 5);
    var pagefull = location.href.substring(location.href.lastIndexOf('/') + 1);//, location.href.indexOf('.aspx') + 5
    var breadcrumbs = '';
    $('#menumain').find('a[href^="'+pagename+'"]').parents('li')
        .each(function(){
            $(this).addClass('over');
            breadcrumbs = $(this).find('span.wrap:eq(0)').html() + (breadcrumbs == '' ? '' : (' > ' + breadcrumbs));
        });
    $('#breadcrumbs').html(breadcrumbs);
});
</script>
