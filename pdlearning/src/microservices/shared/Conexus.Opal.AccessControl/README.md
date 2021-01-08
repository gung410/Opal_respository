# Access Control

## Data migration guideline

To use access control feature in local enviroment, you must migrate data from SAM module into microservices.
Following steps below:

### 1. Create SAM database in your local environment

Create a new database named `db_sam_local`. This database will be used in next steps.

### 2. Import users, departments, hierarchy departments data to your local server

Steps:

* Export SAM database as sql script from developemnt environment.
    - Use [Generate Script...](https://docs.microsoft.com/en-us/sql/ssms/scripting/generate-scripts-sql-server-management-studio?view=sql-server-2017) feature from SQL Server Management Studio Tool
    - In case you can't do it or you want do it faster, you can download script was generated from [this link](https://trongthect-my.sharepoint.com/personal/chu_nguyen_orientsoftware_com/_layouts/15/download.aspx?UniqueId=776f54ee%2D1d70%2D4675%2D9088%2Dafb175464ffd)
* Import data into your SQL Server by this cli:
    ```bash
    $  sqlcmd -d db_sam_local -i <your_script_file_downloaded.sql> -S <YOUR_SERVER_NAME> -U <your_MSSQL_user> -P <your_MSSQL_password>
    ```
