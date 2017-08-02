/*************************
 * Original url: http://wiki.unity3d.com/index.php/LanguageFilter
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/LanguageFilter.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * Author: Aubrey Falconer (Robur) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.UtilityScripts
{
    DescriptionWant to include a chat system in your game? Running user inputs through this script should help to keep things a little more family friendly... 
    LanguageFilter.jsstatic function Filter(str : String) {
    	var patternMild = /*mildwords filter to right ---> */																																	"crap|prawn|d4mn|damn";
    	str = Regex.Replace(str, patternMild, ".", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    	var pattern = /*badwords filter to right ---> */																																		"anus|ash0le|ash0les|asholes| ass |Ass Monkey|Assface|assh0le|assh0lez|bastard|bastards|bastardz|basterd|suka|asshole|assholes|assholz|asswipe|azzhole|bassterds|basterdz|Biatch|bitch|bitches|Blow Job|blowjob|in bed|butthole|buttwipe|c0ck|c0cks|c0k|Clit|cnts|cntz|cockhead| cock |cock-head|CockSucker|cock-sucker|cum|cunt|cunts|cuntz|dick|dild0|dild0s|dildo|dildos|dilld0|dilld0s|dominatricks|dominatrics|dominatrix|f.u.c.k|f u c k|f u c k e r|fag|fag1t|faget|fagg1t|faggit|faggot|fagit|fags|fagz|faig|faigs|fuck|fucker|fuckin|mother fucker|fucking|fucks|Fudge Packer|fuk|Fukah|Fuken|fuker|Fukin|Fukk|Fukkah|Fukken|Fukker|Fukkin|gay|gayboy|gaygirl|gays|gayz|God-dam|God dam|h00r|h0ar|h0re|jackoff|jerk-off|jizz|kunt|kunts|kuntz|Lesbian|Lezzian|Lipshits|Lipshitz|masochist|masokist|massterbait|masstrbait|masstrbate|masterbaiter|masterbate|masterbates|Motha Fucker|Motha Fuker|Motha Fukkah|Motha Fukker|Mother Fucker|Mother Fukah|Mother Fuker|Mother Fukkah|Mother Fukker|mother-fucker|Mutha Fucker|Mutha Fukah|Mutha Fuker|Mutha Fukkah|Mutha Fukker|orafis|orgasim|orgasm|orgasum|oriface|orifice|orifiss|packi|packie|packy|paki|pakie|peeenus|peeenusss|peenus|peinus|pen1s|penas|penis|penis-breath|penus|penuus|Phuc|Phuck|Phuk|Phuker|Phukker|polac|polack|polak|Poonani|pr1c|pr1ck|pr1k|pusse|pussee|pussy|puuke|puuker|queer|queers|queerz|qweers|qweerz|qweir|recktum|rectum|retard|sadist|scank|schlong|screwing|sex | sex|sh1t|sh1ter|sh1ts|sh1tter|sh1tz|shit|shits|shitter|Shitty|Shity|shitz|Shyt|Shyte|Shytty|Shyty|skanck|skank|skankee| sob |skankey|skanks|Skanky|slut|sluts|Slutty|slutz|son-of-a-bitch|turd|va1jina|vag1na|vagiina|vagina|vaj1na|vajina|vullva|vulva|xxx|b!+ch|bitch|blowjob|clit|arschloch|fuck|shit|asshole|b!tch|b17ch|b1tch|bastard|bi+ch|boiolas|buceta|c0ck|cawk|chink|clits|cum|cunt|dildo|dirsa|ejakulate|fatass|fcuk|fuk|fux0r|l3itch|lesbian|masturbate|masterbat*|motherfucker|s.o.b.|mofo|nigga|nigger|n1gr|nigur|niiger|niigr|nutsack|phuck|blue balls|blue_balls|blueballs|pussy|scrotum|shemale|sh!t|slut|smut|teets|tits|boobs|b00bs|testical|testicle|titt|jackoff|whoar|whore|fuck|shit|arse|bi7ch|bitch|bollock|breasts|cunt|dick|fag|feces|fuk|futkretzn|gay|jizz|masturbat*|piss|poop|porn|p0rn|pr0n|shiz|splooge|b00b|testicle|titt|wank";
    	return Regex.Replace(str, pattern, "#", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
}
}
