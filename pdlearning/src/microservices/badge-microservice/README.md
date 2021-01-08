# Badge microservice

This microservice is applied C# 9.0 but not all features
The list below is notes for features that are NOT allowed to apply:

-   New syntax of using statement.
    Avoid: 
    ```csharp
        public void DoBadUsing()
        {
            using var httpClient = new HttpClient();
            // Do whatever;
        }
    ```
    Should: 
    ```csharp
        public void DoGoodUsing()
        {
            using (var httpClient = new HttpClient())
            {
                // Do whatever;
            }
        }
    ```
