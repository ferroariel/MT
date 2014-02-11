
var providers_large = {
	google : {
		name : 'Google',
		url : 'https://www.google.com/accounts/o8/id'
	},
	yahoo : {
		name : 'Yahoo',
		url : 'http://me.yahoo.com/'
    },
    live : {
		name : 'LiveID',
		url: 'https://login.live.com/wlogin.srf?appid=00000000400E961A&alg=wsignin1.0&lc=1033'
	}	
};

var providers_small = {};

openid.locale = 'es';
openid.sprite = 'es'; // reused in german& japan localization
openid.demo_text = '';
openid.signin_text = 'Ingresar';
openid.image_title = 'Ingresar usando {provider}';
