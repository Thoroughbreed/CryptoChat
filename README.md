![Build succeeded][build-shield]
![Test passing][test-shield]
[![Issues][issues-shield]][issues-url]
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![License][license-shield]][license-url]
# CryptoChat

### What is this?
CryptoChat is a lightweight client/server application with end-to-end encrypted chat.

### Commands
Besides the menu, you have the following commands at your use:
- :q! - Quit dammit!
- ls - lists current users in the room
- mv - If you just type mv, you move room, but if you input arguments like `mv NewName` you rename yourself
- pwd - If you just type pwd you can change your secure key (hidden), if you add arguments like `pdw NewPassword` it changes to NewPassword

#### Known issues
When you build the project, VS sometimes claims that the class ChatRoom is static, however it is not static. It looks like a bug in the protobuf generated class, this is easily fixed with the editor suggestion of making ChatRoom non static.

### Changelog
| Version | Change |
|-|-|
| 0.0.1 | First release, smoke test |
| 0.1.0 | Chat works! |
| 0.1.2 | Multiple people can join |
| 0.1.3 | Now with sessions |
| 0.2.0 | Now with encryption |
| 0.2.5 | Works with multiple users |
| 0.5.0 | UI added to the client, possibility to change name, password etc. |
| 1.0.0 | First *real* release, just some code cleanup |
<p align="right">(<a href="#top">back to top</a>)</p>

### Roadmap
- [x] Build the base of the application
- [x] Enable multiple clients chatting
- [x] Enable private chat
- [x] Test encryption
- [x] Implement end-to-end encryption
- [x] Create a console-UI
- [ ] Create an application (UI)
- [ ] Create a web frontend
- [ ] Further expansions?


### License
* Software: Apache 2.0
<p align="right">(<a href="#top">back to top</a>)</p>


### Contact
Jan Andreasen - jan@tved.it

[![Twitter][twitter-shield]][twitter-url]

Project Link: [https://github.com/jaa2019/CryptoChat](https://github.com/jaa2019/CryptoChat)
<p align="right">(<a href="#top">back to top</a>)</p>


<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[build-shield]: https://img.shields.io/badge/Build-success-brightgreen.svg
[test-shield]: https://img.shields.io/badge/Tests-pass-brightgreen.svg
[contributors-shield]: https://img.shields.io/github/contributors/jaa2019/CryptoChat.svg?style=badge
[contributors-url]: https://github.com/jaa2019/CryptoChat/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/jaa2019/CryptoChat.svg?style=badge
[forks-url]: https://github.com/jaa2019/CryptoChat/network/members
[issues-shield]: https://img.shields.io/github/issues/jaa2019/CryptoChat.svg?style=badge
[issues-url]: https://github.com/jaa2019/CryptoChat/issues
[license-shield]: https://img.shields.io/github/license/jaa2019/CryptoChat.svg?style=badge
[license-url]: https://github.com/jaa2019/CryptoChat/blob/master/LICENSE.txt
[twitter-shield]: https://img.shields.io/twitter/follow/andreasen_jan?style=social
[twitter-url]: https://twitter.com/andreasen_jan
