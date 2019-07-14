# SeqPowerK
SeqPowerK is a simulation software made for my paper,"The statistical power of k-mer based aggregative statistics for alignment-free detection of horizontal gene transfer", and it is used to calculate the statistical power of k-mer based sequence statistics.  
Only support Windows.
# Usage
![](https://github.com/liuxuemeiscut/SeqPowerK/blob/master/3.PNG)
There are 5 parts on the interface:Sequence,CRMS,Setting,Save,and State.   
### Sequence: Set up Sequence Simulation
Red area:You can set the Number of simulated cycles，Random seed，Sequence Length.  
Blue area:The probability of bases in simulation is set here. (1/4Mode--e.i.i.d        1/3Mode--n.i.i.d)
![](https://github.com/liuxuemeiscut/SeqPowerK/blob/master/4.PNG)     
If "Divide Sequence" is selected,there will be the orange area to set the sequence subsampled.        
Windows:Fragment Length         
Period:Gap Length
### CRMS: Set the Method to Generate the Background Sequence
CRMS:Cis-regulatory Module  
Replace:Horizontal Gene Transfer  
Possibility:Bernoulli Probability of Module Insertion
### Setting: Alignment-free Statistic and Relevant Parameters
Black area:Parameters among k(k-mer length) and M(Markov order)        
Green area:Alignment-free Statistic
### Save: Set the Save Path
### State: Dispaly the Progress of Calculation.
