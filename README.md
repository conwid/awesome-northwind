This repo contains a curated collection of Northwind sample databases for different database engines.

## Northwind for SQL Server
Not much to tell about this one, this is almost the same as the original database. I did add some minor modifications on my own, though.

Right at the beginning of the script, thereare two calls to the `sp_dboption` stored procedure. Well, this has been deprecated for a while in newer versions of SQL Server, so if you run the script, you get warnings. It's not a big deal, but who likes warnings? So I changed the call to the stored procedure to the corresponding `ALTER DATABASE` calls:

```sql
ALTER DATABASE Northwind SET RECOVERY SIMPLE
ALTER DATABASE Northwind SET RECOVERY BULK_LOGGED
```

The other thing that I changed was the structure of the joining tables (OrderDetail and EmployeeTerritory). These had composite keys composed of the two foreign keys that they contained. I hate composite keys and any keys that are naturally part of the record. There are a couple of reasons (data cannot be changed, indices become fragmented and [you have to defragment](https://dotnetfalcon.com/azure-automation-job-for-index-maintenance/) ). So I added a single new field with an identity, auto-increment values to these tables and populated the values. Oh, and I also removed the whitespace from the "Order Details" table (I mean, come on).

## Northwind for SQL Azure
Azure SQL doesn't support a lot of features of the on-premise SQL Server, so the script above cannot be used. I removed everything from the script corresponding to unavailable features or option, but it's essentially the same as the one for the on-premise server.

## Northwind for SQLite
A lot of time I had to teach at companies where the IT was very... overwhelmed with other tasks, so they didn't have the time to setup SQL Server for the students. For the courses about SQL Server or Entity Framework, this can be problematic :) But for WPF or ASP.NET courses where you just need a data source to display something on the pages (of course, a cloud-based service is not an option because in these cases usually network traffic is also blocked :) ).

In these situations SQLite can be a real life-saver (or an XML file, but that's very hard to handle when it comes to inserting, updating or modifying data). So I created a port for SQLite as well. Of course some of the data types are not supported (like money, or datetime are stored as strings) and unfortunately SQLite has no concept of stored procedures, so those are gone, sorry. And the best thing: the whole database is just a file, of course :)

## Northwind for Azure CosmosDB
This is the one that I'm very proud of :) When you create an Azure SQL database, you have the option to populate the database with sample data (AdventureWorks of course), but you have no such option for CosmosDB. And this makes it very inconvenient to get started with this technology. So I decided to create a port for CosmosDB as well. Since this is not a relational database, I had to make a couple of changes. 

Of course I could have just created one collection for each table and just migrate the data as json, but that would have been a cop out &mdash; not to mention expensive as hell. So first I compressed the schema to be a little more non-relational:
* Shippers and suppliers have their own entity
* Categories have their own entity, and products are embedded
* Customers have their own entity, orders are embedded in customers and order details are embedded in orders, while order details reference products with a "foreign key"
* Regions have their own entity and territories are embedded
* EmployeeTerritories have their own entities and reference territories and employees with "foreign keys"

This makes it a little more NoSQL-like. But to place every entity to a different collection would still be very expensive. This is a common problem with CosmosDB and comes up very often. The solution that Microsoft recommends is to put as many "entities" as possible to the same collection and add an extra field that denotes the entity type. Since every field is indexed by default, you can efficiently query based on this field (or you can later even partition based on this). So everything goes into one collection :)

Stored procedures are also migrated (except for one, because I was lazy). Again, stored procedures are inherently different in CosmosDB, so I had to make a lot of changes (a lot of things are not supported, unfortunately). Views are not supported, so those are gone, sorry.

To populate the database, you can use the [migration tool](https://docs.microsoft.com/en-us/azure/cosmos-db/import-data) and the json files from the Github repo. To create the stored procedures, you can use the text file and copy the bodies of the stored procedures to the Azure portal, or use the C# project that I added to the repository to add each of them automatically.

### Contribution

If you have any ideas to improve these databases or have a new database engine to port the database to, feel free to create an issue or submit the PR.