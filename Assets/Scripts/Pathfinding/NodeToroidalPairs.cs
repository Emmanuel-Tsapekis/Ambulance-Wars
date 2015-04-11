using UnityEngine;
using System.Collections;

public class NodeToroidalPair {

	public Node node1;
	public Node node2;

	public bool containsNodes (Node node1, Node node2) {
		if((node1 == this.node1 && node2 == this.node2) || (node2 == this.node1 && node1 == this.node2)){
			return true;
		}
		else{
			return false;
		}
	}


}
