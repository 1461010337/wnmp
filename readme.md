Windows, Nginx, MySQL & PHP(Wnmp)
=================================
Wnmp values security, stability and user friendliness.
------------------------------------------------------


### Versions of the software(Updated: June 6th 2013): ######

  * Nginx 1.5.1

  * MariaDB 5.5.31

  * PHP 5.4.16 (Non Thread Safe + FastCGI)

  * phpMyAdmin 4.0.3
### How to Install ######

  1. To install download the latest version of Wnmp [here][1] (latest version 2.0.2.3)
  2. Then open *Wnmp.exe* and install it anywhere.
  3. And then run *Wnmp.exe*(which is located in the Wnmp folder)
  4. And then press the Start all button.
  5. And thats it, enjoy Wnmp!


----

### Notes ######

The Username and Password for MySQL is: user: *root* pass *password* (I recommend changing the password)

You should change $cfg['blowfish_secret'] in config.inc.php in the phpmyadmin folder for added security.

The SSl Certificate is Self Signed by me. For added security create your own.

[![Download][3]][1]
[![Download][4]][2]

[Website](http://wnmp.x64architecture.com)

If you can, please donate to [kurt@x64architecture.com][2] using paypal for my effort. All donations are appreciated no matter if big or small. 

----

### System Requirements ######
-------------------------------------------------
 Program        | Amount of ram 	
-------------------------------------------------
 Nginx		    | 8-12 MB of ram		
-------------------------------------------------
 PHP FastCGI    | 15-20 MB of ram		
-------------------------------------------------
 Nginx		    | 512 MB of ram		
-------------------------------------------------

----

### FAQ ######

###### Why Does MariaDB take up so much ram? 
It takes up so much ram vs all of the other programs because MariaDB works very well with that configuration. And you can buy 1GB of ram for very cheap.

###### Why use Wnmp over WAMP?
You should use Wnmp over WAMP because Apache is very inefficient. Read more [here][5]. Also there is another reason you should use Wnmp, Wnmp values security, stability and user friendliness. And Wnmp always keeps up to date with the latest Nginx, MariaDB, PHP and phpMyAdmin releases.

###### When will I update this program?
When some of the components get updated to a stable release, if there is a security vulnerability, And if the GUI has bugs or feature enhancements.

###### I have a question or need some help?
[https://groups.google.com/forum/#!forum/windows-nginx-mysql-php-discuss][6]

[1]: https://code.google.com/p/windows-nginx-mysql-php/downloads/detail?name=Wnmp%202.0.2.3.exe
[2]: https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=P7LAQRRNF6AVE
[3]: https://i1.wp.com/www.akmodding.com/wp-content/uploads/2012/08/akdlbutton.png
[4]: https://s0.wp.com/imgpress?url=http%3A%2F%2Fs1.softpedia-static.com/base_img/softpedia_free_award_f.gif
[5]: http://www.wikivs.com/wiki/Apache_vs_nginx
[6]: https://groups.google.com/forum/#!forum/windows-nginx-mysql-php-discuss