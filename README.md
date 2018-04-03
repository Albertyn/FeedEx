# FeedEx
Twitter feed programming exercise

<table>
  <tr><td>
<img src="https://docs.microsoft.com/en-us/dotnet/images/hub/netcore.svg" width="100">
  </td><td>
<img src="https://docs.microsoft.com/en-us/dotnet/images/hub/csharp.svg" width="100">
  </td></tr>
</table>

### Prerequisite
Download & Install [dotnet-core-2.0](https://www.microsoft.com/net/download/Windows/run) runtime for your environment

#### Runnig the code
* Download or clone the repository.
* Edit the **appsettings.json** file to indicate the location of input files *Users.txt* and *Tweets.txt*
* Open a command line & navigate to the folder containing the repository.
* restore dependencies, buid the app and run it

Command Line
```
C:\FeedExUnzip\dotnet restore
C:\FeedExUnzip\cd FeedEx
C:\FeedExUnzip\FeedEx\dotnet run
```
### Expected Output
* List all users found in Users.txt alphabetically and for each user in the list
  * Print the "name"
  * Print all "tweets" posted by the user as well as users he/she follows in the order they are found in the file Tweets.txt

#### Assumptions
a batch of tweets need to be redistributed to a list of unique users 
(clients => determine if and what you should push to view)
the list of (clients) is the product of a function applied to input *Users.txt* 

##### Users.txt
1. User names are unique strings,
2. Contain **NO spaces** and 
3. have a *minimum length of 1(one) character*.
4. The word 'follows' appears once per line; even if the user does not follow anyone.


the list helps with 'if'. 
Output user tweets 'if any' and 'if' the user follows anyone that posted a tweet in this batch of *Tweets.txt*. Then push tweets or else logic. To lookup tweets an index is generated while validating the content (length) after it is loaded. Same data but filestream boxed into an enumeranle collection to query.
##### Tweets.txt
1. Contains *NO blank or empty* tweets.
2. All tweets are by users found in **Users.txt**
3. Do not have to trim() whitespace for tweet. 140 char.
4. If Tweets.txt contain messages by other Users not found in Users.txt they **will be ignored**.

  *valid test cases*
  * Do all the lines of the file contain "well formed" data 
  * Does the sut handle the exeption.
  * Does it contain any users/tweets at all 
  * Do they follow anyone

On the use of Tuples vs. POCO classes : Tuples for performance / poco's for readable code.
Also thanks to ...
```C#
using System.Collections.Generic;
using System.Linq;
```
:octocat:
