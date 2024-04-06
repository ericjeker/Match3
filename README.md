# Match 3

## Description

Match 3 Candy Crush Like Game in Unity, created based on the tutorial by git-amend:

- [Part 1](https://www.youtube.com/watch?v=ErHEZ5YGQ5M&list=PLnJJ5frTPwRPn4cVet2VRDcsSdvpXdWFt&index=1)
- [Part 2](https://www.youtube.com/watch?v=s6lqTI8dOQ4&list=PLnJJ5frTPwRPn4cVet2VRDcsSdvpXdWFt&index=2)
- [Part 3](https://www.youtube.com/watch?v=SThzZ_5-erM&list=PLnJJ5frTPwRPn4cVet2VRDcsSdvpXdWFt&index=3)

## Differences

There are a few things that I will make differently as it seems that the original tutorial is a bit outdated
or the coding principles applied were not the best. This is obviously my personal taste. 

Here are the differences:

1. I extracted all the debugging from the GridSystem and put it in a separate Debugging class that is
   attached to an empty GameObject in the scene. This way, the GridSystem is cleaner and more focused
2. I will probably extract the Converters from the GridSystem and put them in a separate Converters class. 
   The reason is the same as the previous point but also because pseudo-namespace is not a good practice in C#.

## Evolution

I will evolve this project by adding more features and making it more complex. The idea is to make it
a full game that can be published on the Google Play Store.

