# Introduction
Thank you for taking the time to do the coding challenge. The goal of the challenge is to assess your level of engineering seniority.<br/>
The base challenge should take you 2 hours, but we would like to encourage you to spend more time on it, so we can better assess your level.

# What would we like to see
The coding challenge will test your skills to convert functional requirements into a technical solution, taking into account things that are not explicitly mentioned (just like a real life situation where the stakeholder does not exactly know what he/she wants).<br/><br/>

Furthermore, we expect you to write the solution in such a way that you are proud of the code. It should be ready to go out to production, and the code should be in such a state that it can be shared with other developers to be worked on. Think about general software engineering practices, patterns and code refactoring techniques.<br/><br/>

It is important to note that we would like to see different skills. We don't want you to waste time on repetitive tasks. If you have proven that you can do it once or twice, we know you can do it multiple times (Just make sure you mention it with a comment in the code why you didn't do it. This shows that you have thought about it). 

# The code
This repository contains a simple web API that has some endpoints that are not (fully) implemented. You are free to do anything you like to the solution, except for changing the contracts and endpoints of the API (as we will be running some automated tests).<br/><br/>

Additionally, we have added some comments in the code indicating bugs or small challenge we would like to solve. Please leave the comment, but you can do anything underneath it to solve the bug. Additionally, please provide what you think the problem was and why you think your change solved it.<br/><br/>

Example: <br/>
"// TODO-001: Challenge: Something is wrong with this code. What is it and how can you solve it"<br/>
"// Answer: We are looping over the same array twice, while we could have done it once. Looping over it twice is neglectable, but still, looping once is faster."

# Functional requirements
I have a successful online ordering system where I sell products.<br/>
To promote my website even more I want to be able to give away coupons that can be applied to certain products by sending coupon codes over email.<br/>
When those coupons are applied to a product, I want the product price to change based on how it is defined on the coupon.<br/><br/>

For Example:<br/>
I am selling toy cars for kids. Two of those cars have product code "TRUCK" and "AMBU" and they both cost €20.00.<br/> 
I want to be able to create a coupon that can be applied to both products where I can set the price to €10.00 euro instead.<br/><br/>

Furthermore, some coupons I am handing out can only be used once or a few times.<br/><br/>

For Example:<br/>
If I have a coupon that can be applied on a product. This coupon is a one-time use coupon.<br/>
As soon as an order comes in that contains this coupon, nobody else should be able to use the coupon again.<br/><br/>

# Technical Requirements
You can assume that this service is hosted in an environment that is not public to the world. <br/>
However, everyone that is on the office network (through VPN) does have access.<br/>
We don't want to make it to complex, so everyone that has an API key is allowed to hit the endpoint (if they are in the office).<br/><br/>

My online ordering system is being used by quite a lot of users. This will mean that my service will scale and create extra nodes.<br/>
We need to make sure that this will not lead to problems with limited-time use coupons.

# Scope
This service is part of a big system. You only have to think about this service and you don't have to worry about the other systems.<br/><br/>

You can assume:<br/>
1. Another service will call the GET endpoint to retrieve the coupon and expose it to the front end (API Gateway)
2. Another service will call the PUT endpoint to create a coupon before sending it to the customers as an email (The Email Service)
3. Another service will call the PUT endpoint when the use count of a coupon needs to be updated. (The Order Service)
4. Another service will check if the product the coupon can be applied on is in their basket. It will not call the Coupon service if the coupon is in the basket but no product to apply it to. (The Pricing Service)
5. Another service will reject the order if a coupon is requested that cannot be used anymore. (The Pricing Service)
6. Another service will update the price of the product when the coupon is applied. (The Pricing Service)

# Steps
1. Analyze the functional & Technical requirements
2. Analyze the scope
3. Analyze the existing code
4. Come up with an idea on how you want to structure your code (again, as long as the endpoints and contracts remain the same, you can do anything)
5. Implement a solution that will satisfy the functional requirements
6. Search for all "// TODO" in the code and solve them
7. Provide a document to describe:
    1. Anything that you think needs clarifications on the provided solution.
    2. Anything that you would have done if you had more time.