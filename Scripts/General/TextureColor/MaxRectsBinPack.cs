/*************************
 * Original url: http://wiki.unity3d.com/index.php/MaxRectsBinPack
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/TextureColor/MaxRectsBinPack.cs
 * File based on original modification date of: 8 May 2012, at 13:46. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.TextureColor
{
    Description Author : Jukka Jylänki's ( http://clb.demon.fi/projects/more-rectangle-bin-packing ) 
    A shortcoming of the GUILLOTINE data structure is that the split planes fragment the free area of the bin into disjoint rectangles, and new placements cannot straddle these split lines. The MAXRECTS data structure removes this limitation by tracking all the maximal rectangles of the free area remaining in the bin. 
    The maximal rectangles will not be disjoint, so extra care is needed to maintain consistency of the data structure when placing new rectangles. In the paper, I argue that the number of maximal rectangles in a rectilinear polygon is at most 2n^2, which gives a worst case O(n^5) time complexity for the whole algorithm. This holds if the rectangles were placed arbitrarily, but since we are placing the rectangles as conservatively as possible, the number of maximal rectangles is far less. In practice, I have observed that the number of maximal rectangles is strictly sublinear, which lends to an average-case n^3 algorithm. 
    The MAXRECTS data structure can be used to implement two interesting heuristics: 
    1) The Contact Point heuristics. In this rule, the new rectangle is placed to a position where its edge touches the edges of previously placed rectangles as much as possible. 
    2) The Bottom Left heuristics. This rule effectively implements the commonly cited Tetris method for rectangle packing: Each rectangle is placed to a position (possibly rotating it) where its top side lies as low as possible. 
    Both of these variants perform very well. For the example input, the MAXRECTS-BL-BNF variant achieved an occupancy rate of 90.92%, and the MAXRECTS-CP-BNF variant got an occupancy rate of 93.35%. However, the best performance was given by the MAXRECTS-BSSF-BNF algorithm. The occupancy rate in the example was 94.06%. 
    Note : this method cannot be used for building atlases if they require mip-mapping due to it not currently supporting padding (this may be added in the near future). In that case, see nVidia Texture Atlas Tools. In fact, remember to disable the automatic mip chain generation whenever you're building atlases of this type. 
    
    Edit : Modified the code so you can turn off rotations. 
    C# - MaxRectsBinPack .cs  
    /*
     	Based on the Public Domain MaxRectsBinPack.cpp source by Jukka Jylänki
     	https://github.com/juj/RectangleBinPack/
     
     	Ported to C# by Sven Magnus
     	This version is also public domain - do whatever you want with it.
    */
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
     
     
    public class MaxRectsBinPack {
     
    	public int binWidth = 0;
    	public int binHeight = 0;
    	public bool allowRotations;
     
    	public List<Rect> usedRectangles = new List<Rect>();
    	public List<Rect> freeRectangles = new List<Rect>();
     
    	public enum FreeRectChoiceHeuristic {
    		RectBestShortSideFit, ///< -BSSF: Positions the rectangle against the short side of a free rectangle into which it fits the best.
    		RectBestLongSideFit, ///< -BLSF: Positions the rectangle against the long side of a free rectangle into which it fits the best.
    		RectBestAreaFit, ///< -BAF: Positions the rectangle into the smallest free rect into which it fits.
    		RectBottomLeftRule, ///< -BL: Does the Tetris placement.
    		RectContactPointRule ///< -CP: Choosest the placement where the rectangle touches other rects as much as possible.
    	};
     
    	public MaxRectsBinPack(int width, int height, bool rotations = true) {
    		Init(width, height, rotations);
    	}
     
    	public void Init(int width, int height, bool rotations = true) {
    		binWidth = width;
    		binHeight = height;
    		allowRotations = rotations;
     
    		Rect n = new Rect();
    		n.x = 0;
    		n.y = 0;
    		n.width = width;
    		n.height = height;
     
    		usedRectangles.Clear();
     
    		freeRectangles.Clear();
    		freeRectangles.Add(n);
    	}
     
    	public Rect Insert(int width, int height, FreeRectChoiceHeuristic method) {
    		Rect newNode = new Rect();
    		int score1 = 0; // Unused in this function. We don't need to know the score after finding the position.
    		int score2 = 0;
    		switch(method) {
    			case FreeRectChoiceHeuristic.RectBestShortSideFit: newNode = FindPositionForNewNodeBestShortSideFit(width, height, ref score1, ref score2); break;
    			case FreeRectChoiceHeuristic.RectBottomLeftRule: newNode = FindPositionForNewNodeBottomLeft(width, height, ref score1, ref score2); break;
    			case FreeRectChoiceHeuristic.RectContactPointRule: newNode = FindPositionForNewNodeContactPoint(width, height, ref score1); break;
    			case FreeRectChoiceHeuristic.RectBestLongSideFit: newNode = FindPositionForNewNodeBestLongSideFit(width, height, ref score2, ref score1); break;
    			case FreeRectChoiceHeuristic.RectBestAreaFit: newNode = FindPositionForNewNodeBestAreaFit(width, height, ref score1, ref score2); break;
    		}
     
    		if (newNode.height == 0)
    			return newNode;
     
    		int numRectanglesToProcess = freeRectangles.Count;
    		for(int i = 0; i < numRectanglesToProcess; ++i) {
    			if (SplitFreeNode(freeRectangles[i], ref newNode)) {
    				freeRectangles.RemoveAt(i);
    				--i;
    				--numRectanglesToProcess;
    			}
    		}
     
    		PruneFreeList();
     
    		usedRectangles.Add(newNode);
    		return newNode;
    	}
     
    	public void Insert(List<Rect> rects, List<Rect> dst, FreeRectChoiceHeuristic method) {
    		dst.Clear();
     
    		while(rects.Count > 0) {
    			int bestScore1 = int.MaxValue;
    			int bestScore2 = int.MaxValue;
    			int bestRectIndex = -1;
    			Rect bestNode = new Rect();
     
    			for(int i = 0; i < rects.Count; ++i) {
    				int score1 = 0;
    				int score2 = 0;
    				Rect newNode = ScoreRect((int)rects[i].width, (int)rects[i].height, method, ref score1, ref score2);
     
    				if (score1 < bestScore1 || (score1 == bestScore1 && score2 < bestScore2)) {
    					bestScore1 = score1;
    					bestScore2 = score2;
    					bestNode = newNode;
    					bestRectIndex = i;
    				}
    			}
     
    			if (bestRectIndex == -1)
    				return;
     
    			PlaceRect(bestNode);
    			rects.RemoveAt(bestRectIndex);
    		}
    	}
     
    	void PlaceRect(Rect node) {
    		int numRectanglesToProcess = freeRectangles.Count;
    		for(int i = 0; i < numRectanglesToProcess; ++i) {
    			if (SplitFreeNode(freeRectangles[i], ref node)) {
    				freeRectangles.RemoveAt(i);
    				--i;
    				--numRectanglesToProcess;
    			}
    		}
     
    		PruneFreeList();
     
    		usedRectangles.Add(node);
    	}
     
    	Rect ScoreRect(int width, int height, FreeRectChoiceHeuristic method, ref int score1, ref int score2) {
    		Rect newNode = new Rect();
    		score1 = int.MaxValue;
    		score2 = int.MaxValue;
    		switch(method) {
    			case FreeRectChoiceHeuristic.RectBestShortSideFit: newNode = FindPositionForNewNodeBestShortSideFit(width, height, ref score1, ref score2); break;
    			case FreeRectChoiceHeuristic.RectBottomLeftRule: newNode = FindPositionForNewNodeBottomLeft(width, height, ref score1, ref score2); break;
    			case FreeRectChoiceHeuristic.RectContactPointRule: newNode = FindPositionForNewNodeContactPoint(width, height, ref score1); 
    				score1 = -score1; // Reverse since we are minimizing, but for contact point score bigger is better.
    				break;
    			case FreeRectChoiceHeuristic.RectBestLongSideFit: newNode = FindPositionForNewNodeBestLongSideFit(width, height, ref score2, ref score1); break;
    			case FreeRectChoiceHeuristic.RectBestAreaFit: newNode = FindPositionForNewNodeBestAreaFit(width, height, ref score1, ref score2); break;
    		}
     
    		// Cannot fit the current rectangle.
    		if (newNode.height == 0) {
    			score1 = int.MaxValue;
    			score2 = int.MaxValue;
    		}
     
    		return newNode;
    	}
     
    	/// Computes the ratio of used surface area.
    	public float Occupancy() {
    		ulong usedSurfaceArea = 0;
    		for(int i = 0; i < usedRectangles.Count; ++i)
    			usedSurfaceArea += (uint)usedRectangles[i].width * (uint)usedRectangles[i].height;
     
    		return (float)usedSurfaceArea / (binWidth * binHeight);
    	}
     
    	Rect FindPositionForNewNodeBottomLeft(int width, int height, ref int bestY, ref int bestX) {
    		Rect bestNode = new Rect();
    		//memset(bestNode, 0, sizeof(Rect));
     
    		bestY = int.MaxValue;
     
    		for(int i = 0; i < freeRectangles.Count; ++i) {
    			// Try to place the rectangle in upright (non-flipped) orientation.
    			if (freeRectangles[i].width >= width && freeRectangles[i].height >= height) {
    				int topSideY = (int)freeRectangles[i].y + height;
    				if (topSideY < bestY || (topSideY == bestY && freeRectangles[i].x < bestX)) {
    					bestNode.x = freeRectangles[i].x;
    					bestNode.y = freeRectangles[i].y;
    					bestNode.width = width;
    					bestNode.height = height;
    					bestY = topSideY;
    					bestX = (int)freeRectangles[i].x;
    				}
    			}
    			if (allowRotations && freeRectangles[i].width >= height && freeRectangles[i].height >= width) {
    				int topSideY = (int)freeRectangles[i].y + width;
    				if (topSideY < bestY || (topSideY == bestY && freeRectangles[i].x < bestX)) {
    					bestNode.x = freeRectangles[i].x;
    					bestNode.y = freeRectangles[i].y;
    					bestNode.width = height;
    					bestNode.height = width;
    					bestY = topSideY;
    					bestX = (int)freeRectangles[i].x;
    				}
    			}
    		}
    		return bestNode;
    	}
     
    	Rect FindPositionForNewNodeBestShortSideFit(int width, int height, ref int bestShortSideFit, ref int bestLongSideFit)  {
    		Rect bestNode = new Rect();
    		//memset(&bestNode, 0, sizeof(Rect));
     
    		bestShortSideFit = int.MaxValue;
     
    		for(int i = 0; i < freeRectangles.Count; ++i) {
    			// Try to place the rectangle in upright (non-flipped) orientation.
    			if (freeRectangles[i].width >= width && freeRectangles[i].height >= height) {
    				int leftoverHoriz = Mathf.Abs((int)freeRectangles[i].width - width);
    				int leftoverVert = Mathf.Abs((int)freeRectangles[i].height - height);
    				int shortSideFit = Mathf.Min(leftoverHoriz, leftoverVert);
    				int longSideFit = Mathf.Max(leftoverHoriz, leftoverVert);
     
    				if (shortSideFit < bestShortSideFit || (shortSideFit == bestShortSideFit && longSideFit < bestLongSideFit)) {
    					bestNode.x = freeRectangles[i].x;
    					bestNode.y = freeRectangles[i].y;
    					bestNode.width = width;
    					bestNode.height = height;
    					bestShortSideFit = shortSideFit;
    					bestLongSideFit = longSideFit;
    				}
    			}
     
    			if (allowRotations && freeRectangles[i].width >= height && freeRectangles[i].height >= width) {
    				int flippedLeftoverHoriz = Mathf.Abs((int)freeRectangles[i].width - height);
    				int flippedLeftoverVert = Mathf.Abs((int)freeRectangles[i].height - width);
    				int flippedShortSideFit = Mathf.Min(flippedLeftoverHoriz, flippedLeftoverVert);
    				int flippedLongSideFit = Mathf.Max(flippedLeftoverHoriz, flippedLeftoverVert);
     
    				if (flippedShortSideFit < bestShortSideFit || (flippedShortSideFit == bestShortSideFit && flippedLongSideFit < bestLongSideFit)) {
    					bestNode.x = freeRectangles[i].x;
    					bestNode.y = freeRectangles[i].y;
    					bestNode.width = height;
    					bestNode.height = width;
    					bestShortSideFit = flippedShortSideFit;
    					bestLongSideFit = flippedLongSideFit;
    				}
    			}
    		}
    		return bestNode;
    	}
     
    	Rect FindPositionForNewNodeBestLongSideFit(int width, int height, ref int bestShortSideFit, ref int bestLongSideFit) {
    		Rect bestNode = new Rect();
    		//memset(&bestNode, 0, sizeof(Rect));
     
    		bestLongSideFit = int.MaxValue;
     
    		for(int i = 0; i < freeRectangles.Count; ++i) {
    			// Try to place the rectangle in upright (non-flipped) orientation.
    			if (freeRectangles[i].width >= width && freeRectangles[i].height >= height) {
    				int leftoverHoriz = Mathf.Abs((int)freeRectangles[i].width - width);
    				int leftoverVert = Mathf.Abs((int)freeRectangles[i].height - height);
    				int shortSideFit = Mathf.Min(leftoverHoriz, leftoverVert);
    				int longSideFit = Mathf.Max(leftoverHoriz, leftoverVert);
     
    				if (longSideFit < bestLongSideFit || (longSideFit == bestLongSideFit && shortSideFit < bestShortSideFit)) {
    					bestNode.x = freeRectangles[i].x;
    					bestNode.y = freeRectangles[i].y;
    					bestNode.width = width;
    					bestNode.height = height;
    					bestShortSideFit = shortSideFit;
    					bestLongSideFit = longSideFit;
    				}
    			}
     
    			if (allowRotations && freeRectangles[i].width >= height && freeRectangles[i].height >= width) {
    				int leftoverHoriz = Mathf.Abs((int)freeRectangles[i].width - height);
    				int leftoverVert = Mathf.Abs((int)freeRectangles[i].height - width);
    				int shortSideFit = Mathf.Min(leftoverHoriz, leftoverVert);
    				int longSideFit = Mathf.Max(leftoverHoriz, leftoverVert);
     
    				if (longSideFit < bestLongSideFit || (longSideFit == bestLongSideFit && shortSideFit < bestShortSideFit)) {
    					bestNode.x = freeRectangles[i].x;
    					bestNode.y = freeRectangles[i].y;
    					bestNode.width = height;
    					bestNode.height = width;
    					bestShortSideFit = shortSideFit;
    					bestLongSideFit = longSideFit;
    				}
    			}
    		}
    		return bestNode;
    	}
     
    	Rect FindPositionForNewNodeBestAreaFit(int width, int height, ref int bestAreaFit, ref int bestShortSideFit) {
    		Rect bestNode = new Rect();
    		//memset(&bestNode, 0, sizeof(Rect));
     
    		bestAreaFit = int.MaxValue;
     
    		for(int i = 0; i < freeRectangles.Count; ++i) {
    			int areaFit = (int)freeRectangles[i].width * (int)freeRectangles[i].height - width * height;
     
    			// Try to place the rectangle in upright (non-flipped) orientation.
    			if (freeRectangles[i].width >= width && freeRectangles[i].height >= height) {
    				int leftoverHoriz = Mathf.Abs((int)freeRectangles[i].width - width);
    				int leftoverVert = Mathf.Abs((int)freeRectangles[i].height - height);
    				int shortSideFit = Mathf.Min(leftoverHoriz, leftoverVert);
     
    				if (areaFit < bestAreaFit || (areaFit == bestAreaFit && shortSideFit < bestShortSideFit)) {
    					bestNode.x = freeRectangles[i].x;
    					bestNode.y = freeRectangles[i].y;
    					bestNode.width = width;
    					bestNode.height = height;
    					bestShortSideFit = shortSideFit;
    					bestAreaFit = areaFit;
    				}
    			}
     
    			if (allowRotations && freeRectangles[i].width >= height && freeRectangles[i].height >= width) {
    				int leftoverHoriz = Mathf.Abs((int)freeRectangles[i].width - height);
    				int leftoverVert = Mathf.Abs((int)freeRectangles[i].height - width);
    				int shortSideFit = Mathf.Min(leftoverHoriz, leftoverVert);
     
    				if (areaFit < bestAreaFit || (areaFit == bestAreaFit && shortSideFit < bestShortSideFit)) {
    					bestNode.x = freeRectangles[i].x;
    					bestNode.y = freeRectangles[i].y;
    					bestNode.width = height;
    					bestNode.height = width;
    					bestShortSideFit = shortSideFit;
    					bestAreaFit = areaFit;
    				}
    			}
    		}
    		return bestNode;
    	}
     
    	/// Returns 0 if the two intervals i1 and i2 are disjoint, or the length of their overlap otherwise.
    	int CommonIntervalLength(int i1start, int i1end, int i2start, int i2end) {
    		if (i1end < i2start || i2end < i1start)
    			return 0;
    		return Mathf.Min(i1end, i2end) - Mathf.Max(i1start, i2start);
    	}
     
    	int ContactPointScoreNode(int x, int y, int width, int height) {
    		int score = 0;
     
    		if (x == 0 || x + width == binWidth)
    			score += height;
    		if (y == 0 || y + height == binHeight)
    			score += width;
     
    		for(int i = 0; i < usedRectangles.Count; ++i) {
    			if (usedRectangles[i].x == x + width || usedRectangles[i].x + usedRectangles[i].width == x)
    				score += CommonIntervalLength((int)usedRectangles[i].y, (int)usedRectangles[i].y + (int)usedRectangles[i].height, y, y + height);
    			if (usedRectangles[i].y == y + height || usedRectangles[i].y + usedRectangles[i].height == y)
    				score += CommonIntervalLength((int)usedRectangles[i].x, (int)usedRectangles[i].x + (int)usedRectangles[i].width, x, x + width);
    		}
    		return score;
    	}
     
    	Rect FindPositionForNewNodeContactPoint(int width, int height, ref int bestContactScore) {
    		Rect bestNode = new Rect();
    		//memset(&bestNode, 0, sizeof(Rect));
     
    		bestContactScore = -1;
     
    		for(int i = 0; i < freeRectangles.Count; ++i) {
    			// Try to place the rectangle in upright (non-flipped) orientation.
    			if (freeRectangles[i].width >= width && freeRectangles[i].height >= height) {
    				int score = ContactPointScoreNode((int)freeRectangles[i].x, (int)freeRectangles[i].y, width, height);
    				if (score > bestContactScore) {
    					bestNode.x = (int)freeRectangles[i].x;
    					bestNode.y = (int)freeRectangles[i].y;
    					bestNode.width = width;
    					bestNode.height = height;
    					bestContactScore = score;
    				}
    			}
    			if (allowRotations && freeRectangles[i].width >= height && freeRectangles[i].height >= width) {
    				int score = ContactPointScoreNode((int)freeRectangles[i].x, (int)freeRectangles[i].y, height, width);
    				if (score > bestContactScore) {
    					bestNode.x = (int)freeRectangles[i].x;
    					bestNode.y = (int)freeRectangles[i].y;
    					bestNode.width = height;
    					bestNode.height = width;
    					bestContactScore = score;
    				}
    			}
    		}
    		return bestNode;
    	}
     
    	bool SplitFreeNode(Rect freeNode, ref Rect usedNode) {
    		// Test with SAT if the rectangles even intersect.
    		if (usedNode.x >= freeNode.x + freeNode.width || usedNode.x + usedNode.width <= freeNode.x ||
    			usedNode.y >= freeNode.y + freeNode.height || usedNode.y + usedNode.height <= freeNode.y)
    			return false;
     
    		if (usedNode.x < freeNode.x + freeNode.width && usedNode.x + usedNode.width > freeNode.x) {
    			// New node at the top side of the used node.
    			if (usedNode.y > freeNode.y && usedNode.y < freeNode.y + freeNode.height) {
    				Rect newNode = freeNode;
    				newNode.height = usedNode.y - newNode.y;
    				freeRectangles.Add(newNode);
    			}
     
    			// New node at the bottom side of the used node.
    			if (usedNode.y + usedNode.height < freeNode.y + freeNode.height) {
    				Rect newNode = freeNode;
    				newNode.y = usedNode.y + usedNode.height;
    				newNode.height = freeNode.y + freeNode.height - (usedNode.y + usedNode.height);
    				freeRectangles.Add(newNode);
    			}
    		}
     
    		if (usedNode.y < freeNode.y + freeNode.height && usedNode.y + usedNode.height > freeNode.y) {
    			// New node at the left side of the used node.
    			if (usedNode.x > freeNode.x && usedNode.x < freeNode.x + freeNode.width) {
    				Rect newNode = freeNode;
    				newNode.width = usedNode.x - newNode.x;
    				freeRectangles.Add(newNode);
    			}
     
    			// New node at the right side of the used node.
    			if (usedNode.x + usedNode.width < freeNode.x + freeNode.width) {
    				Rect newNode = freeNode;
    				newNode.x = usedNode.x + usedNode.width;
    				newNode.width = freeNode.x + freeNode.width - (usedNode.x + usedNode.width);
    				freeRectangles.Add(newNode);
    			}
    		}
     
    		return true;
    	}
     
    	void PruneFreeList() {
    		for(int i = 0; i < freeRectangles.Count; ++i)
    			for(int j = i+1; j < freeRectangles.Count; ++j) {
    				if (IsContainedIn(freeRectangles[i], freeRectangles[j])) {
    					freeRectangles.RemoveAt(i);
    					--i;
    					break;
    				}
    				if (IsContainedIn(freeRectangles[j], freeRectangles[i])) {
    					freeRectangles.RemoveAt(j);
    					--j;
    				}
    			}
    	}
     
    	bool IsContainedIn(Rect a, Rect b) {
    		return a.x >= b.x && a.y >= b.y 
    			&& a.x+a.width <= b.x+b.width 
    			&& a.y+a.height <= b.y+b.height;
    	}
     
    }
}
