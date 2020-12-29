# Spreadsheet Analysis 
Research papers and codebase exploring spreadsheet formulas as computer programs.

Formula rich spreadsheets are fascinatingly complex computer programs developed by people who would not consider themselves “programmers”. This repository contains the research code behind two papers exploring the intricacy of such spreadsheets. I hope it may be of use to others exploring these artefacts. This work would not have been possible without the collaborators and funders who are gratefully in the two papers below. Both papers were presented at The European Spreadsheet Risk Interest Group Conference ([EuSpRig](http://www.eusprig.org/)). The slides for both presentations are linked below and serve as a good introduction to these works.  

## Abstracting spreadsheet data flow through hypergraph redrawing 
David Birch1, Nicolai Stawinoga1, Jack Binks2, Bruno Nicoletti2, Paul Kelly1

1 Imperial College London 2 Filigree Technologies

We believe the error prone nature of traditional spreadsheets is due to their low level of abstraction. End user programmers are forced to construct their data models from low level cells which we define as “a data container or manipulator linked by user-intent to model their world and positioned to reflect its structure”. Spreadsheet cells are limited in what they may contain (scalar values) and the links between them are inherently hidden. This paper proposes a method of raising the level of abstraction of spreadsheets by “redrawing the boundary” of the cell.
To expose the hidden linkage structure we transform spreadsheets into fine-grained graphs with operators and values as nodes. “cells” are then represented as hypergraph edges by drawing a boundary “wall” around a set of operator/data nodes. To extend what cells may contain and to create a higher level model of the spreadsheet we propose that researchers should seek techniques to redraw these boundaries to create higher level “cells” which will more faithfully represent the end-user’s real world/mental model. We illustrate this approach via common sub-expression identification and the application of sub-tree isomorphisms for the detection of vector (array) operations
### Links: 
 - [Paper](https://github.com/davidbirchwork/SpreadsheetAnalysis/blob/main/Papers/2019/EUSPRIG_2019_Abstracting_spreadsheet_data_flow.pdf)
 - [Slides](https://github.com/davidbirchwork/SpreadsheetAnalysis/blob/main/Papers/2019/Eusprig_2019_Slides.pdf)
 - Code see **ExcelCoreAnalytics.sln** which runs on .net core and is thus cross platform. 
 
![Cell level graph of a typical model from the construction industry to answer a commercial question and exploring
the effect of different build completion dates.](https://github.com/davidbirchwork/SpreadsheetAnalysis/raw/main/Papers/2019/2019_ThumbB.png)

## Multidisciplinary Engineering Models: Methodology and Case Study in Spreadsheet Analytics
David Birch a *, Helen Liang b, Paul H J Kelly a, Glen Mullineux b,
Tony Field a, Joan Ko c, Alvise Simondetti c

a Imperial College London b University of Bath c Arup

This paper demonstrates a methodology to help practitioners maximise the utility of complex
multidisciplinary engineering models implemented as spreadsheets, an area presenting unique
challenges. As motivation we investigate the expanding use of Integrated Resource Management
(IRM) models which assess the sustainability of urban masterplan designs. IRM models reflect the
inherent complexity of multidisciplinary sustainability analysis by integrating models from many
disciplines. This complexity makes their use time-consuming and reduces their adoption.
We present a methodology and toolkit for analysing multidisciplinary engineering models
implemented as spreadsheets to alleviate such problems and increase their adoption. For a given
output a relevant slice of the model is extracted, visualised and analysed by computing model and
interdisciplinary metrics. A sensitivity analysis of the extracted model supports engineers in their
optimisation efforts. These methods expose, manage and reduce model complexity and risk whilst
giving practitioners insight into multidisciplinary model composition. We report application of the
methodology to several generations of an industrial IRM model and detail the insight generated,
particularly considering model evolution.

### Links: 
 - [Paper](https://github.com/davidbirchwork/SpreadsheetAnalysis/blob/main/Papers/2013/2013_EAM_Multidisciplinary_Engineering_Models.pdf)
 - [Slides](https://github.com/davidbirchwork/SpreadsheetAnalysis/blob/main/Papers/2013/EUSPRIG_2013_Slides.pdf)
 - Code see **ExcelAnalytics.sln**
 - This codebase contains a modified version of [Ncalc](https://archive.codeplex.com/?p=ncalc). Parts of the GUI require [Infralution Virtual Tree v3.15](http://www.infralution.com/downloads.html)

![Identifying a spreadsheet bug through graph visualization](https://github.com/davidbirchwork/SpreadsheetAnalysis/raw/main/Papers/2013/2013_Thumb.png)
