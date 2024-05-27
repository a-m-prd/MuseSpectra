ID: T01
Description: Register
Prerequisite: The program is in the login window.
Test steps: 	1. Click on “Register”
		2. A new name is entered in the “Username” field.
		3. A password is entered in the “Password” field.
		4. A new email address is entered in the “Email” field.
		5. Click on “Register”.
Expected result: MessageBox “Congratulations {username}! Your registration was successful.” appears. Visualization window opens, user is logged in and has been created.

ID: T02
Description: Login
Prerequisite: The program is in the login window.
Test steps: 	1. The name of an account is entered in the “Username” field.
		2. The appropriate password is entered in the “Password” field.
		3. Click on “Login”.
Expected result: MessageBox “Welcome {user.Username}!” appears. Visualizati-on window opens, user is logged in.
 
ID: T03
Description: Customize user interface
Prerequisite: The program is in the visualization window.
Test steps: 1. Click on “Hide Parameters”
Expected result: The parameters have been hidden and the user interface now only shows the visualization.

ID: T04
Description: Represent music
Prerequisite: The program is in the visualization window.
Test steps: 1. User plays audio.
Expected result: Program displays audio in the appropriate format.

ID: T05
Description: Customize visualization
Prerequisite: The program is in the visualization window.
Test steps: 	1. Settings are made.
		2. Click on “Apply”.
Expected result: Visualization changes accordingly.
 
ID: T06
Description: Start the program with Windows
Prerequisite: The program is in the visualization window.
Test steps: 	1. Click on “Settings”.
		2. Click on the “Start with Windows” checkbox.
Expected result: MessageBox “Settings saved. Please restart the application for changes to take effect.” appears. The program now starts when Windows starts.


ID: T07
Description: Save data
Prerequisite: The program was used by the user.
Test steps: 	1. Log in.
		2. Check whether settings have been saved.
Expected result: Settings from the last session are saved and applied at startup.
