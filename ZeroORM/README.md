# New Features in 0.0.7v
## Intercept Deserialize Process:
Mapping functions of `SqlDataReader` has got new action delegate `Action<T, SqlDataReader> postSet` which allow you to add your custom mapping code after default mapping of ZeroOrm is finished, and here are list of helper extensions that are useful when mapping `SqlDataReader` into business object
1. `SetValue(this, expression, dbColumnName, reader)` useful when you database column name is diffrent from business object property name, so you refere to property using expression and provide the dbColumnName explecitly.
2. `SetValue(this, expression, value)` useful when you need to manipulate the value type before set it into business object, so you refer to property using expression and provide the manipulated value expelcitly.
3. `SetValue(this, expression, reader)` userfull to map database column from reader into property explicilty, nowing that this is handled implicilty by ZeroOrm.

## Enrich Mapping Function:
New overload versions of mapping function for `SqlDataReader` has been added to provide much control over how to deserialize `SqlDataReader`, Sync and Async functions, Single Row and Multiple Rows, Create new list or add to existing list instance, and much more capabilities been added.
1. `ReadEach(reader, action, postSet)`, `ReadEach(reader, action)` Provide the ability to deserialize row of `SqlDataReader` into instance of business object then pass it to delegate action parameter which allow you to execute custom logic on each materialized business object of `SqlDataReader` row.
2. `SetListAsync(reader, entities, postSet, cancellationToken)`, `SetListAsync(reader, entities, cancellationToken)` mapping rows of SqlDataReader into business object and filling up the provided list with them. This allow you to use your list instance to have all rows in it.
3. `ToListAsync(reader, postSet, cancellationToken)`, `ToListAsync(reader, cancellationToken)` asynchronouslly create new List instance with all rows of SqlDataReader after mapping them into business object.
4. `ToList(reader, postSet)`, `ToList(reader)` create new List instance with all rows of SqlDataReader after mapping them into business object.
5. `ToEntityAsync(reader, postSet, cancellationToken)`, `ToEntityAsync(reader, cancellationToken)` Asynchrounusly mapping `SqlDataReader` values into new instance of business object, this method cal `ReadAsync(canncelationToken)` method of `SqlDataReader` before start mapping.
6. `ToEntity(reader, postSet)`, `ToEntity(reader)` Mapping `SqlDataReader` values into new instance of business object, this method call `Read()` method of `SqlDataReader` before start mapping.
7. `SetEntity(reader, entity, postSet)`, `SetEntity(reader, entity)` Mapping `SqlDataReader` values into specified instance of business object, this method DOESN'T call `Read()` method so make sure to call it before calling it to avoid exception.

## Implement Sync overloads ZeroOrm functions
Synchronous versions are still required especially when you need to work with database using lock methods in order to update tables and records in sequence.
1. `ExecuteNonQuery` Executes a Transact-SQL statement against the connection and returns number of rows affected.
2. `ExecuteScalar` Executes the query, and returns first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
3. `ExecuteReader` Sends the `commandText` to the `connectionString` and builds a `SqlDataReader`.

## Intercept Parametrizing Process
Parametrizing business object into `SqlParameters` is one important feature of ZeroOrm and now got new function delegate `Func<T, IEnumerable<SqlParameter>> parmMap` which allow you to add your custom parametriation code after default parametrizing of ZeroOrm is finished and the parameters that generated using this delegate are the ones that will be sent to Sql Server, here are list of useful helper function you can use
1. `ToParameter(item, expression, dbColumnName)` useful when business object property name is different than the parameter name in commandText.
2. `ToParameter(item, expression)` useful when you need to map business object property into the same name of SqlParameter knowing that this is done implicilty by ZeroOrm.
3. `ToLikeParameter(item, expression)` useful when you need to map business object property into Like Sql Operator, this is subject to improvement in future.