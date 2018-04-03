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
The expected output is for each user found in Users.txt, to print the Username as well as all the tweets from the user as well as all the users they follow.

### Expected Output
* List all users found in Users.txt alphabetically and for each user in the list
  * Print the "name"
  * Print all "tweets" posted by the user as well as users he/she follows in the order they are found in the file Tweets.txt

#### Assumptions
    
##### Users.txt
1. User names are unique strings,
2. Contain **NO spaces** and 
3. have a *minimum length of 1(one) character*.
4. The word 'follows' appears once per line; even if the user does not follow anyone.

##### Tweets.txt
1. Contains *NO blank or empty* tweets.
2. All tweets are by users found in **Users.txt**
3. Do not have to trim() whitespace for tweet. 140 char.
4. If Tweets.txt contain messages by other Users not found in Users.txt they **will be ignored**.

On the use of Tuples vs. POCO classes : Tuples for performance / poco's for readable code.
Also thanks to ...
```C#
using System.Collections.Generic;
using System.Linq;
```
:octocat:
