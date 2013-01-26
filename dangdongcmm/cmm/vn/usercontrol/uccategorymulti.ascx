<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uccategorymulti.ascx.cs" Inherits="dangdongcmm.cmm.uccategorymulti" %>

<div class="rowhor">
    <div class="title"><span class="require">*</span> Thuộc danh mục:</div>
    <div class="boxctrll"><div class="categorymulti"><ASP:LITERAL ID="ltrCategory" runat="server"></ASP:LITERAL></div></div>
</div>


<script type="text/javascript">
$(document).ready(function() {
    $('.categorymulti').find('input[type="checkbox"]')
        .each(function(){
            $(this).change(function() {
                if( $(this).attr('checked') ) {
                    $(this).parents('li')
                        .each(function(){
                            $(this).find('input[type="checkbox"]:first').attr('checked', true);
                        });
                }    
            });
            
        });
});
</script>
