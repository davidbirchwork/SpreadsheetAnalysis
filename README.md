# Spreadsheet Analysis 
Research papers and codebase exploring spreadsheet formulas as computer programs.

Formula rich spreadsheets are fascinatingly complex computer programs developed by people who would not consider themselves “programmers”. This repository contains the research code behind two papers exploring the intricacy of such spreadsheets. I hope it may be of use to others exploring these artefacts. This work would not have been possible without the collaborators and funders who are gratefully in the two papers below. The slides presenting both papers are included and serve as a good introduction to this work.  

## Abstracting spreadsheet data flow through hypergraph redrawing 
David Birch1, Nicolai Stawinoga1, Jack Binks2, Bruno Nicoletti2, Paul Kelly1

1 Imperial College London 2 Filigree Technologies

We believe the error prone nature of traditional spreadsheets is due to their low level of abstraction. End user programmers are forced to construct their data models from low level cells which we define as “a data container or manipulator linked by user-intent to model their world and positioned to reflect its structure”. Spreadsheet cells are limited in what they may contain (scalar values) and the links between them are inherently hidden. This paper proposes a method of raising the level of abstraction of spreadsheets by “redrawing the boundary” of the cell.
To expose the hidden linkage structure we transform spreadsheets into fine-grained graphs with operators and values as nodes. “cells” are then represented as hypergraph edges by drawing a boundary “wall” around a set of operator/data nodes. To extend what cells may contain and to create a higher level model of the spreadsheet we propose that researchers should seek techniques to redraw these boundaries to create higher level “cells” which will more faithfully represent the end-user’s real world/mental model. We illustrate this approach via common sub-expression identification and the application of sub-tree isomorphisms for the detection of vector (array) operations
### Please see ExcelCoreAnalytics.sln for this codebase.

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

### Please see ExcelAnalytics.sln for this codebase.
