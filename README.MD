Refactoring exercise

- as the first step, I performed migration to dotNet6. Besides obvious benefits, I was forced to this step as, at this moment, I have only access to macOS workstations

- even with the recognition that refactoring of the code with low test coverage (or in this case) introduces significant risk, the fact that the code was functionally broken, prevented mitigation of that risk by the initial increase of test coverage. 

- the first step/task was to make code testable (during further refactoring, unit tests are introduced)

- the goal of the next few steps was to make code more readable - some compiler warnings were fixed, and code duplications were removed...

- the next big step was the introduction of the mediator pattern - the controller is now simple and has a single responsibility

- further progress is made by improving folder/namespace infrastructure (improvement in readability) and additional removal of duplicated code.

- even as simple as it is, the Logger implementation is also identified as a possible place for improvement. IMHO if it is a well-known, supported, available 3rd party implementation, it must be considered. In most cases, usage will increase maintainability and lower the complexity of the solution. The Microsoft Logger was chosen to replace internal implementation. 

- so far, the main focus was on Shop.WebApi as it is the main part of the project. As VendorWebApi is just a mock of existing third party API, we have certain limitations - first and foremost, the exposed interface must be preserved to realistically mimic real-world api. Also, as this part is only mock (there is no intention to grow the production system), it "deserves" significantly less attention. API was transformed to "minimal". The code was cleaned. The introduction of unit testing is considered an overshot due to the nature of api.

- once again, the code is cleaned in several swaps.

- finally, an additional trace log is placed on strategic points, which increases the possibility of a better monitor system. API is "documented".