### How to Run API:
    1. Compile the project. Use command in terminal: dotnet publish -c Release -o MSTU_API
    2. Start the project. Use command: dotnet MSTUApi.dll
    
    
### How to Use API:
    
    Status Codes:
     0 - OK
    -1 - Wrong input data
    -2 - Unknown server error
    
    [ GET ] Controller
      Methods:
        groupList    -    No additional data needed
        timeTable    -    url (string) or group (string)
        week         -    url (string) or group (string)
        
      
      
      Examples:
        groupList
          Request:
            https://example.com/get/groupList
          Response:
            {"Status":0,"Data":[{"GroupName":"БМТ1-22Б", "Url":"https://students.bmstu.ru/schedule/d2e864a2-4aee-11e9-8c40-005056960017"}, {...}], "Message":null}
        
        timeTable
          Request:
            https://example.com/get/timeTable?url=https://students.bmstu.ru/schedule/d2e864a2-4aee-11e9-8c40-005056960017
		        https://example.com/get/timeTable?group=БМТ1-22Б
          Response:
            {"Status":0,"Message":null,"Data":[{"Lections":[{"Time":"08:30 - 10:05","Numerator":null,"Denominator":null,"Both":"(лек) Линейная алгебра и функции нескольких переменных 224л Грибов А. Ф."}]}
          
          week
            Request:
              https://example.com/get/timeTable?group=БМТ1-22Б
              https://example.com/get/timeTable?url=https://students.bmstu.ru/schedule/d2e864a2-4aee-11e9-8c40-005056960017
            Response:
              {"Status":0,"Message":null,"Data":"7 неделя, числитель"}
    
    [ CHECK ] Controller
      Methods:
        group       -    name (string)
      
      Examples:
        group
          Request:
            https://example.com/check/group?name=БМТ1-22Б
          Response:
            {"Status":0,"Message":null,"Data":{"GroupName":"БМТ1-22Б","isRight":true}}
           
        
