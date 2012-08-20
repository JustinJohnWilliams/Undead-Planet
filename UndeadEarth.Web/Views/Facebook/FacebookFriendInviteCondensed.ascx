<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<!-- pull down Facebook's Javascript SDK -->
<script src="http://connect.facebook.net/en_US/all.js"></script>

<div id="fb-root">
    <fb:serverfbml width="650px">
        <script type="text/fbml">
            <fb:fbml>
            	<fb:request-form
                    action="http://www.undeadearththegame.com/Facebook/AddFacebookFriend/" 
                    method="post" type="Invite" content="Killing zombies.  Saving the world.  Join the fight. <fb:req-choice label='Undead Planet' url='http://apps.facebook.com/undeadplanet/' />">
          			<fb:multi-friend-selector condensed="true" style="width: 650px" max="100" showborder="false" actiontext="Invite Friends to Undead Planet" email_invite="true" bypass="cancel" />
                    <fb:request-form-submit label="Send Invite" />
            	</fb:request-form> 
            </fb:fbml>
        </script>
    </fb:serverfbml>
</div>
        
<script type="text/javascript">
    FB.init({
        apiKey: 'cf82966b32133fea84edcb7a30ad4a58',
        status: true, // check login status
        cookie: true, // enable cookies to allow the server to access the session
        xfbml: false  // parse XFBML 
    });

    FB.Event.subscribe('xfbml.render', function () { $('iframe').css('width', 650); });
    FB.getLoginStatus(handleSessionResponse);

    function handleSessionResponse(response) {
        FB.XFBML.parse();
    }

</script>

