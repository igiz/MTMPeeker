

# TFSPeekerDesktop

This is a desktop console version of a tool which is able to look into MTM test suites and plans to display them in various other views , such as all the unassigned cases etc. This is a C# project , so just download the solution and compile as per usual.
It comes with some bundled views which are defined in TestCaseViewFactory.GetView(name) method. Examples should be easy enough to work out how to create your own views. Now the tool can also be configured via a JSON file so you don't have to provide the console arguments all the time. Just place a config.json file next to the executable. The following is the configuration format:

     {   
    	 "tfsUrl":"https://some-url-to-tfs.com/DefaultCollection",  
    	 "project": "SomeProject",  
    	 "testSuiteId": 1,   
    	 "testPlanId": 1,
    	 "views": "priority-2-unassigned",
		 "pollingTimeInMinutes": 5
      }

 - **tfsUrl** - The url to TFS hosting MTM.
 - **project** - The project that test suite cases are hosted on.
 - **testSuiteId** - Is the id of the test suite in MTM.
 - **testPlanId** - Is the instance of the plan in the test suite.
 - **views** - this is a comma seperated list of the views you want the console application to generate. These have to match the names of the views specified in TestCaseViewFactory.GetView(name)
 - **pollingTimeInMinutes** - This is the poll time in minutes on when to refresh the given views.
