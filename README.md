# Introduction
Thank you for taking the time to do the coding challenge. The goal of the challenge is to assess your engineering skills.  
The base challenge should take you 2 hours, but we would like to encourage you to spend more time on it, so we can better assess your level.
If you are running out of time, please make sure that you write down what you would have done if you had more time. Even if you haven't finished, please submit the code you have written so far. We will still review it and provide feedback.

When you submit your solution, please specify how much time you spent on your solution.

# What would we like to see
The coding challenge will test your skills to convert functional requirements into a technical solution, taking into account things that are not explicitly mentioned (just like a real life situation where the stakeholder does not exactly know what he/she wants).    

Furthermore, we expect you to write the solution in such a way that you are proud of the code. It should be ready to go out to production, and the code should be in such a state that it can be shared with other developers to be worked on. Think about general software engineering practices, patterns and code refactoring techniques.    

It is important to note that we would like to see different skills. We don't want you to waste time on repetitive tasks. If you have proven that you can do it once or twice, we know you can do it multiple times (Just make sure you mention it with a comment in the code why you didn't do it. This shows that you have thought about it). 

# The code
This repository contains a simple web API that has some endpoints that are not (fully) implemented. You are free to do anything you like to the solution, except for changing the contracts and endpoints of the API (as we will be running some automated tests).    

### Optional Challenges
Additionally, we have added some comments in the code indicating bugs or small challenges we would like to solve. Please don't remove the comment, but you can do anything underneath it to solve the bug. Additionally, please provide what you think the problem was and why you think your change solved it.    

Example:   
"// TODO-001: Challenge: Something is wrong with this code. What is it and how can you solve it"  
"// Answer: We are looping over the same array twice, while we could have done it once. Looping over it twice is neglectable, but still, looping once is faster."

Note that this is an optional step. If you don't have time to do it, try to just write a little bit of an explanation of what you think the problem is and how you would solve it.

# Functional requirements
I have a successful online ordering system where I sell products.  
To promote my website even more I want to be able to give away coupons that can be applied to certain products by sending coupon codes over email.  
When those coupons are applied to a product, I want the product price to change based on how it is defined on the coupon.    

For Example:  
I am selling toy cars for kids. Two of those cars have product codes "TRUCK" and "AMBU" and they both cost �20.00.   
I want to be able to create a coupon that can be applied to both products where I can set the price to �10.00 euro instead.    

Furthermore, some coupons I am handing out can only be used once or a few times.    

For Example:  
If I have a coupon that can be applied on a product. This coupon is a one-time use coupon.  
As soon as an order comes in that contains this coupon, nobody else should be able to use the coupon again.    

# Technical requirements
You can assume that this service is hosted in an environment that is not public to the world.   
However, everyone that is on the office network (through VPN) does have access.  
We don't want to make it to complex, so everyone that has an API key is allowed to hit the endpoint (if they are in the office).    

My online ordering system is being used by quite a lot of users. This will mean that my service will scale and create extra nodes.  
We need to make sure that this will not lead to problems with limited-time use coupons.    

Some implementation details:  
1. When MaxUsages is set to 0, it means that the coupon can be applied an infinite number of times.
2. Usages can only be incremented / decremented by 1 at a time.
3. The coupon code is unique and immutable once created.
4. The coupon codes are case insensitive.
5. The product codes are case insensitive. 

# Scope
This service is part of a big system. You only have to think about this service and you don't have to worry about the other systems.    

You can assume:  
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