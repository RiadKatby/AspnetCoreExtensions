# AspnetCoreExtensions
Extensions for AspNetCore classes that is practically proven userfull for Enterprise Applications

# Feature Based Asp.net MVC
This is one of the basic features that I loved, because it is fixing the most prominant structure of MVC as there are differents folders for Models, Views, and Controllers you have to navigate all the times up and down between these folder if you only need to modify simple things

in this Additions you Can Create root foler Named Features and create Supfolder for each feature HomeFeature, AccountFeature etc.
Inside these SubFolder Features you can add Folder for Models and off course folder for Views then locate all needed files with this sub-Folder "FeautreFolder"

Off course structuing you code this way support the Clean Code architecutre which make our unkle bob happy

# Zero ORM
Yet another Micro ORM (Object Relational Mapper) for .NET platform with vision of Zero
1. Zero learning Curve to start with you need no experiance that ADO.NET
2. Zero complexy it is just an extentions overy SqlConnection and SqlDataReader
3. Zero latency it is the fastest ORM among others

## Zero ORM features:
### New Versions of ExecuteSclaerAsync, ExecuteNonQueryAsync, and ExecuteReaderAsync [SqlHelper](https://github.com/RiadKatby/AspnetCoreExtensions/blob/master/ZeroORM/SqlHelper.cs)

They are implemented as extension method on SqlConnection to better accessiblity and all of them take parameter called value, this parameter is the key to understand as it takes four types of values
1. Primitive Values: string, int, double, etc, even nullable values
   in this case this single value will be used as parameter whatever you name it in commandText
2. Object full instance of class
   All parameters defined in commandText will be satisfied from this object's properties based on thier names, if parameter name in commandText in not exeist in object as properties InvalidOperationException will be thrown.
3. SqlParameter object
   Your commandText has to have only on parameter weich have the same name of SqlParamete.ParameterName owtherwise InvalidOperationException will be thrown.
4. IEnummerable<SqlParameter> 
   You commandText has to have same number and names of parameters in this IEnummerable of SqlParameter otherwise InvalidOperationException will be thrown.

### Supper Easy Mappers ToEntityAsync, ToListAsync, SetValues, and SetListAsync [SqlDataReaderExtensions](https://github.com/RiadKatby/AspnetCoreExtensions/blob/master/ZeroORM/SqlDataReaderExtensions.cs)

## Roadmap
1. Create Unit Test
2. Conduct Benchmark with ef core, dapper, and zeroOrm
3. Provide centralized place for all ad-hoc sql statements
