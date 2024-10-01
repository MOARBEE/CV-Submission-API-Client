# Password-API
You have to write an application to submit your CV and the code you are about to write to a
REST API as a Zip file. Before you can do this you first need to authenticate yourself against
another REST API to get the URL where you need to submit your CV. Problem is you forgot
the password or at least you forgot how to spell the password. You know the password is
“password”, or was it “Password”, or even “P@55w0rd” … you just can’t remember but you
know that sometimes you replace the ‘a’ with a ‘@’, your ‘s’ with a ‘5’ and your ‘o’ with a ‘0’
… but not always. “Not a problem…”, you think to yourself because you will also write a bit of
code to generate a dictionary and launch an attack on the WEB API until you are able to
guess the correct password. If your plan works you will get the URL of the REST API where
you need to submit your CV.
1. Write a console application in C#
2. Generate a dictionary file called “dict.txt” containing all the permutations of the word
“password” where any of the characters can be either uppercase, lowercase or the
characters a, s, and o can also be replaced with the characters @, 5 and 0.
3. Using your dictionary file, launch an attack on
http://recruitment.warpdevelopment.co.za/api/authenticate to find the correct password (the
username is John). The API call makes use of the “Basic Authentication” scheme. A
successful authentication will return a unique URL in the response body which you need to
use to upload your CV.
Example Request: GET http://recruitment.warpdevelopment.co.za/api/authenticate
Authorization: Basic QWxhZGRpbjpPcGVuU2VzYW1l
Example Response 1: HTTP/1.1 401 Not Authorized
Example Response 2: HTTP/1.1 200 OK
http://recruitment.warpdevelopment.co.za/api/upload/4324scs2345fds
df14265t354wef25432451455tfacagfwrgh
4. Generate a Zip file containing a copy of your CV in PDF format, and a copy of this
program (only the .cs file) as well as your “dict.txt” file. Then encode your bytes as a Base64
encoded string and submit it as Json with a POST to the URL you received in [3]. The Zip
file must be less than 5MB in size.
Example Request: POST / HTTP/1.1
http://recruitment.warpdevelopment.co.za/api/upload/4324scs2345fds
df14265t354wef25432451455tfacagfwrgh/
{
Data:”b8hwqb8hwqEtt_WH0O7KJtNb-9tFWJgZvFREtt_WH0O7b8hw
qEtt_WH0O7KJtNb-9tFWJgZvFRKJtNb8hwqEtt_WH0O7KJtNb-9tFW
JgZvFRb-9tFWJgZvFR===”
}
Example Response: HTTP/1.1 200 OK
{
Message: “Success”
}
