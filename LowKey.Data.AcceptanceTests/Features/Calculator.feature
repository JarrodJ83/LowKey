Feature: Calculator
![Calculator](https://specflow.org/wp-content/uploads/2020/09/calculator.png)
Simple calculator for adding **two** numbers

Link to a feature: [Calculator](LowKey.Data.AcceptanceTests/Features/Calculator.feature)
***Further read***: **[Learn more about how to generate Living Documentation](https://docs.specflow.org/projects/specflow-livingdoc/en/latest/LivingDocGenerator/Generating-Documentation.html)**

@mytag
Scenario: SQL Single Store Single Tenant
	Given a "sql" datastore for database "master"
	And a single SQL Server tenant
	When a command is executed
	Then then it should be executed on "test" data store